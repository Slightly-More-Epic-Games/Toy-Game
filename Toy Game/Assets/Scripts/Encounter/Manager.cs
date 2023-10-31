using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Encounter {
    public class Manager : MonoBehaviour
    {
        // Encounter.Manager manages turn order, killing creatures, and centrally processing events

        public static Manager instance;

        private Queue<Context> eventsToProcess = new Queue<Context>();

        [System.NonSerialized] public List<Creature> playerAllies = new List<Creature>();
        [System.NonSerialized] public List<Creature> playerEnemies = new List<Creature>();
        public static List<Creature> killedEnemies = new List<Creature>();

        [System.NonSerialized] public Creature currentTurn;
        private int turnNumber;

        [SerializeField] private ItemTab itemTabPrefab;
        [SerializeField] private Transform itemTabs;

        [SerializeField] private PlayerController playerController;
        [SerializeField] private AutoController enemyController;
        [SerializeField] private AutoController allyController;

        [SerializeField] private CreatureVisual creatureVisualPrefab;
        [SerializeField] private CreatureVisual largeCreatureVisualPrefab;
        [SerializeField] private Transform creatureVisuals;

        private int endState;

        public void Start() {
            instance = this;
            killedEnemies.Clear();
            endState = 0;

            // creating the player creature doesnt create a new instance (like it does for enemies), but it does create the visual
            CreateCreature(Game.instance.player, true);

            Map.EncounterNode node = (Map.EncounterNode)Map.Manager.instance.currentNode;

            foreach (Creature creature in node.GetEnemies()) {
                CreateCreature(creature, false);
            }

            // encountuers start on players turn - the turn start event sets currentturn, so it doesnt need to be done here
            AddEventToProcess(new Context(Action.EncounterStart, Game.instance.player, Game.instance.player, 0));
            ProcessEvents();

            AddEventToProcess(new Context(Action.TurnStart, Game.instance.player, Game.instance.player, 0));
            ProcessEvents();
        }

        private void Update() {
            if (currentTurn != null && !currentTurn.isDead) {
                // the controller handles doing things like using items and ending the users turn
                // the players controller is what lets the player click things - enemies use a different subclass of creaturecontroller that uses items automatically
                // this means multiple player controlled creatures wouldnt be too hard to implement
                currentTurn.controller.UpdateTurn(currentTurn, turnNumber);
            }

            if (endState != 0) {
                Game.instance.LoadGameScene(endState == 1 ? Game.GameScene.GameOver : Game.GameScene.EncounterWon);
            }
        }

        public bool CreateCreature(Creature template, bool isAlly) {
            // the position that creature visuals appear in is set by putting empty gameobjects in the desired positions
            // this ensures the creatures never collide, but it does mean theres a limit of creatures per side (9)
            Transform position = GetFirstEmptyChild(creatureVisuals.GetChild(isAlly ? 0 : 1));
            // so if there is no free position, the creature cant be created, and we can just skip doing anything and return a fail
            if (position == null) return false;

            Creature instance;
            if (template == Game.instance.player) {
                // player shouldnt be instantiated, but it does need its controller set
                instance = template;
                instance.controller = playerController;
                playerAllies.Add(instance);
            } else {
                // this code handles player allies, however we didnt end up using this
                instance = Instantiate(template);
                instance.Initialise();
                if (isAlly) {
                    instance.controller = allyController;
                    playerAllies.Add(instance);
                } else {
                    instance.controller = enemyController;
                    playerEnemies.Add(instance);
                }
            }

            // regardless of if the creature is an enemy, it needs a visual - however if its a boss it gets a different looking visual
            instance.creatureVisual = Instantiate(template.isLarge ? largeCreatureVisualPrefab : creatureVisualPrefab, position);
            instance.creatureVisual.Init(instance, playerController);
            return true;
        }

        private Transform GetFirstEmptyChild(Transform parent) {
            foreach (Transform child in parent) {
                if (child.childCount == 0) return child;
            }
            return null;
        }

        public void ProcessEvents() {
            // events are processed in a queue, since some events may cause more events and the order needs to be consistent (which recursion or a stack wouldnt be)
            // all events in the game are represented with a context object, which has a type, source, target, and value

            while (eventsToProcess.Count > 0) {
                Context context = eventsToProcess.Dequeue();
                // turn start and end are one of the few events that the manager itself needs to handle
                if (context.action == Action.TurnStart && context.target.isDead) {
                    context.target = currentTurn;
                }

                // all events are then passed out to each creature, which then pass it out to each of their items etc
                // this is where more events might be created, causing a bit of a chain of events
                foreach (Creature creature in playerAllies) {
                    creature.OnEvent(context);
                }
                foreach (Creature creature in playerEnemies) {
                    creature.OnEvent(context);
                }

                if (context.action == Action.TurnEnd) {
                    TurnEnd(context.target);
                }
            }

            // once all events have been processed, each creature then gets an eventfinished call
            // creatures use this to make sure their health is within reasonable bounds, and to get ready for another event chain
            foreach (Creature creature in playerAllies) {
                creature.EventFinished();
            }
            foreach (Creature creature in playerEnemies) {
                creature.EventFinished();
            }
        }

        private void TurnEnd(Creature newCurrent) {
            // creature deaths are only counted on turn end, instead of the moment a creature reaches 0 health
            playerAllies.RemoveAll(x => x.UpdateDeadness());
            playerEnemies.RemoveAll(x => x.UpdateDeadness());

            // move to the next turn, if the new creature is dead, keep moving forwards in turn order until a creature isnt dead
            currentTurn = newCurrent;
            while (currentTurn.isDead) {
                currentTurn = GetNextCreatureInTurnOrder(currentTurn);
                // in the off chance all creatures are dead, we dont want to get stuck in an infinite loop
                if (currentTurn == newCurrent) {
                    break;
                }
            }

            turnNumber++;

            // endState is used in the update loop to end the encounter
            // if the encounter ended here instead of in the update loop, it would cause issues as the scene would unload while code still needs to reference it
            if (Game.instance.player.UpdateDeadness()) {
                if (endState == 0) {
                    AddEventToProcess(new Context(Action.EncounterEnd, newCurrent, newCurrent, turnNumber));
                }
                //losing takes priority over winning
                endState = 1;
            }

            if (playerEnemies.Count == 0) {
                if (endState == 0) {
                    AddEventToProcess(new Context(Action.EncounterEnd, newCurrent, newCurrent, turnNumber));
                    endState = 2;
                }
            }
        }

        public void CreatureDied(Creature owner, Creature source) {
            // this is called once a creature is definately dead and cant be saved
            AddEventToProcess(new Context(Action.AnyDeath, owner, source, owner.health));
            if (playerEnemies.Contains(owner)) {
                // killed enemies is used to generate item rewards
                killedEnemies.Add(owner);
            }
        }

        public void AddEventToProcess(Context context) {
            if (context.action == Action.TurnEnd) {
                // turn end events need to follow a certain format, so instead of needing items to be responsible for formatting it can get "autocorrected"
                // (likewise turn ends are always followed up by turn starts)
                context.source = currentTurn;
                context.value = turnNumber;
                context.target = GetNextCreatureInTurnOrder(currentTurn);
                eventsToProcess.Enqueue(context);
                eventsToProcess.Enqueue(new Context(Action.TurnStart, context.source, context.target, turnNumber+1));
            } else if (context.action == Action.TurnStart) {
                // similarly, a turn start event is always preceeded by a turn end event
                if (currentTurn == context.target) return;
                eventsToProcess.Enqueue(new Context(Action.TurnEnd, currentTurn, context.target, turnNumber));
                context.value = turnNumber+1;
                eventsToProcess.Enqueue(context);
            } else {
                // and if its any other type of event, it can be addded normally
                eventsToProcess.Enqueue(context);
            }
        }

        private Creature GetNextCreatureInTurnOrder(Creature current) {
            int total = playerAllies.Count+playerEnemies.Count;
            if (total == 0) return current;

            int index = playerAllies.Contains(current) ? playerAllies.IndexOf(current) : playerEnemies.IndexOf(current)+playerAllies.Count;
            index = (index + 1)%total;
            return index >= playerAllies.Count ? playerEnemies[index-playerAllies.Count] : playerAllies[index];
        }

        public ItemTab CreateItemTab() {
            return Instantiate(itemTabPrefab, itemTabs);
        }
    }
}
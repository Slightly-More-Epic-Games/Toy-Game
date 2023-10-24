using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Encounter {
    public class Manager : MonoBehaviour
    {
        public static Manager instance;

        private Queue<Context> eventsToProcess = new Queue<Context>();

        [System.NonSerialized] public List<Creature> playerAllies = new List<Creature>();
        [System.NonSerialized] public List<Creature> playerEnemies = new List<Creature>();
        [System.NonSerialized] public List<Creature> killedEnemies = new List<Creature>();

        [System.NonSerialized] public Creature currentTurn;
        private int turnNumber;

        [SerializeField] private ItemTab itemTabPrefab;
        [SerializeField] private Transform itemTabs;

        [SerializeField] private PlayerController playerController;
        [SerializeField] private AutoController enemyController;
        [SerializeField] private AutoController allyController;

        [SerializeField] private CreatureVisual creatureVisualPrefab;
        [SerializeField] private Transform creatureVisuals;

        public HoverInfo hoverInfo;

        public void Start() {
            instance = this;

            CreateCreature(Game.instance.player, true);

            Map.EncounterNode node = (Map.EncounterNode)Map.Manager.instance.currentNode;

            foreach (Creature creature in node.GetEnemies()) {
                CreateCreature(creature, false);
            }

            AddEventToProcess(new Context(Action.EncounterStart, Game.instance.player, Game.instance.player, 0));
            ProcessEvents();

            AddEventToProcess(new Context(Action.TurnStart, Game.instance.player, Game.instance.player, 0));
            ProcessEvents();
        }

        private void Update() {
            currentTurn.controller.UpdateTurn(currentTurn, turnNumber);
        }

        public bool CreateCreature(Creature template, bool isAlly) {
            Transform position = GetFirstEmptyChild(creatureVisuals.GetChild(isAlly ? 0 : 1));
            if (position == null) return false;
            Creature instance;
            if (template == Game.instance.player) {
                instance = template;
                instance.controller = playerController;
                playerAllies.Add(instance);
            } else {
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
            instance.creatureVisual = Instantiate(creatureVisualPrefab, position);
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
            while (eventsToProcess.Count > 0) {
                Context context = eventsToProcess.Dequeue();
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
            foreach (Creature creature in playerAllies) {
                creature.EventFinished();
            }
            foreach (Creature creature in playerEnemies) {
                creature.EventFinished();
            }
        }

        private void TurnEnd(Creature newCurrent) {
            currentTurn = newCurrent;
            turnNumber++;
            playerAllies.RemoveAll(x => x.UpdateDeadness());
            playerEnemies.RemoveAll(x => x.UpdateDeadness());

            if (Game.instance.player.UpdateDeadness()) {
                Game.instance.LoadGameScene(Game.GameScene.GameOver);
            }

            if (playerEnemies.Count == 0) {
                Game.instance.LoadGameScene(Game.GameScene.EncounterWon);
            }
        }

        public void CreatureDied(Creature owner, Creature source) {
            AddEventToProcess(new Context(Action.AnyDeath, owner, source, owner.health));
            if (playerEnemies.Contains(owner)) {
                killedEnemies.Add(owner);
            }
        }

        public void AddEventToProcess(Context context) {
            if (context.action == Action.TurnEnd) {
                context.source = currentTurn;
                context.value = turnNumber;
                context.target = GetNextCreatureInTurnOrder(currentTurn);
                eventsToProcess.Enqueue(context);
                eventsToProcess.Enqueue(new Context(Action.TurnStart, context.source, context.target, turnNumber+1));
            } else if (context.action == Action.TurnStart) {
                if (currentTurn == context.target) return;
                eventsToProcess.Enqueue(new Context(Action.TurnEnd, currentTurn, context.target, turnNumber));
                context.value = turnNumber+1;
                eventsToProcess.Enqueue(context);
            } else {
                eventsToProcess.Enqueue(context);
            }
        }

        private Creature GetNextCreatureInTurnOrder(Creature current) {
            int index = playerAllies.Contains(current) ? playerAllies.IndexOf(current) : playerEnemies.IndexOf(current)+playerAllies.Count;
            index = (index + 1)%(playerAllies.Count+playerEnemies.Count);
            return index >= playerAllies.Count ? playerEnemies[index-playerAllies.Count] : playerAllies[index];
        }

        public ItemTab CreateItemTab() {
            return Instantiate(itemTabPrefab, itemTabs);
        }
    }
}
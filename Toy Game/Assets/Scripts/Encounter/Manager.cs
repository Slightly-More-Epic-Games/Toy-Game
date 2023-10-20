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

        [System.NonSerialized] public Creature currentTurn;
        private int turnNumber;

        [SerializeField] private ItemTab itemTabPrefab;
        [SerializeField] private Transform itemTabs;

        [SerializeField] private PlayerController playerController;

        [SerializeField] private CreatureVisual creatureVisualPrefab;
        [SerializeField] private Transform creatureVisuals;

        public void Start() {
            instance = this;

            playerAllies.Add(Game.instance.player);
            Game.instance.player.playerController = playerController;

            Map.Encounter node = (Map.Encounter)Game.instance.node;

            foreach (Creature creature in node.enemies) {
                playerEnemies.Add(Instantiate(creature));
            }

            foreach (Creature creature in playerAllies) {
                CreateCreatureVisual(creature, true);
            }

            foreach (Creature creature in playerEnemies) {
                CreateCreatureVisual(creature, false);
            }

            AddEventToProcess(new Context(Action.ENCOUNTER_START, Game.instance.player, Game.instance.player, 0));
            ProcessEvents();

            AddEventToProcess(new Context(Action.TURN_START, Game.instance.player, Game.instance.player, 0));
            ProcessEvents();
        }

        private void Update() {
            currentTurn.UpdateTurn();
        }

        private void CreateCreatureVisual(Creature creature, bool isAlly) {
            creature.creatureVisual = Instantiate(creatureVisualPrefab, creatureVisuals.GetChild(isAlly ? 0 : 1));
            creature.creatureVisual.Init(creature);
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

                if (context.action == Action.TURN_END) {
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
                Debug.Log("GAME OVER");
            }

            if (playerEnemies.Count == 0) {
                Debug.Log("ENCOUNTER WON");
            }
        }

        public void AddEventToProcess(Context context) {
            if (context.action == Action.TURN_END) {
                context.source = currentTurn;
                context.value = turnNumber;
                context.target = GetNextCreatureInTurnOrder(currentTurn);
                eventsToProcess.Enqueue(context);
                eventsToProcess.Enqueue(new Context(Action.TURN_START, context.source, context.target, turnNumber+1));
            } else if (context.action == Action.TURN_START) {
                if (currentTurn == context.target) return;
                eventsToProcess.Enqueue(new Context(Action.TURN_END, currentTurn, context.target, turnNumber));
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
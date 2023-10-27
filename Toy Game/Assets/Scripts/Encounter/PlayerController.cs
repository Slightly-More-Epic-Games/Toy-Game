using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Encounter {
    public class PlayerController : CreatureController
    {
        private bool turnActive;

        private int index = -1;
        private Creature target = null;
        private bool bufferedEnd;        

        [System.NonSerialized] public ItemTab itemTab;

        protected override void Update() {
            base.Update();
            if (!usingItem && index != -1 && target != null) {
                UseItem(index, target);
            }
            if (!usingItem && bufferedEnd) {
                EndTurn();
            }
            if (!usingItem && index == -1) {
                itemTab.Select(-1);
            }
        }

        public override void OnTurnStart(Creature owner) {
            turnActive = true;
            this.index = -1;
            itemTab.SetInteractable(owner, true);
            itemTab.SetFlipTarget(0);
            if (target != null) {
                target.creatureVisual.SetTargeted(true, 1f);
            }
        }

        public override void OnTurnEnd(Creature owner) {
            itemTab.SetInteractable(owner, true);
            itemTab.SetFlipTarget(1);
            itemTab.Select(-1);
            if (target != null) {
                target.creatureVisual.SetTargeted(true, 0.5f);
            }
            turnActive = false;
        }

        public override void OnEncounterStart(Creature owner) {
            owner.imagination = 0;
            itemTab = Manager.instance.CreateItemTab();
            itemTab.Init(owner, this);
            itemTab.Select(-1);
        }

        public override void OnEncounterEnd(Creature owner) {
            owner.triggers.Clear();
        }

        public override void OnEventsFinished(Creature owner) {
            itemTab.UpdateAllIcons(owner);
        }

        public void SelectItem(int index) {
            if (!turnActive) return;

            if (this.index == index) {
                this.index = -1;
            } else {
                this.index = index;
            }

            itemTab.Select(this.index);
        }

        public void SelectCreature(Creature target) {
            if (!turnActive) return;
            if (this.target != null) {
                this.target.creatureVisual.SetTargeted(false, 1f);
                if (this.target == target) {
                    this.target = null;
                    return;
                }
            }
            target.creatureVisual.SetTargeted(true, 1f);
            this.target = target;
        }

        public void UseItem(int index, Creature target) {
            Game.instance.player.UseItem(index, target);
            this.index = -1;
        }

        public void EndTurn() {
            if (usingItem) {
                bufferedEnd = true;
            } else {
                if (Manager.instance.currentTurn != Game.instance.player) return;

                Manager.instance.AddEventToProcess(new Context(Action.TurnEnd, Game.instance.player, Game.instance.player, 0));
                Manager.instance.ProcessEvents();
                bufferedEnd = false;
            }
        }
    }
}
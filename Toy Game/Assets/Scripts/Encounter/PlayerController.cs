using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Encounter {
    public class PlayerController : CreatureController
    {
        private bool turnActive;

        private int index = -1;
        private Creature target = null;

        public ItemTab itemTab;

        public override void OnTurnStart(Creature owner) {
            turnActive = true;
            this.target = null;
            this.index = -1;
            itemTab.SetInteractable(owner, true);
        }

        public override void OnTurnEnd(Creature owner) {
            itemTab.SetInteractable(owner, true);
            turnActive = false;
        }

        public override void OnEncounterStart(Creature owner) {
            itemTab = Manager.instance.CreateItemTab();
            itemTab.Init(owner, this);
        }

        public override void OnEncounterEnd(Creature owner) {
            owner.imagination = 0;
            owner.triggers.Clear();
        }

        public override void OnEventsFinished(Creature owner) {
            itemTab.UpdateAllIcons(owner);
        }

        public void SelectItem(int index) {
            if (!turnActive) return;
            this.index = index;
            if (this.target != null) UseItem(index, target);
        }

        public void SelectCreature(Creature target) {
            if (!turnActive) return;
            this.target = target;
            if (this.index != -1) UseItem(index, target);
        }

        public void UseItem(int index, Creature target) {
            Game.instance.player.UseItem(index, target);
            this.target = null;
            this.index = -1;
        }

        public void EndTurn() {
            if (Manager.instance.currentTurn != Game.instance.player) return;

            Manager.instance.AddEventToProcess(new Context(Action.TurnEnd, Game.instance.player, Game.instance.player, 0));
            Manager.instance.ProcessEvents();
        }
    }
}
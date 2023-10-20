using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Encounter {
    public class PlayerController : MonoBehaviour
    {
        private void Update() {

        }

        private int index = -1;
        private Creature target = null;

        public void SelectItem(int index) {
            if (!gameObject.activeSelf) return;
            this.index = index;
            if (this.target != null) UseItem(index, target);
        }

        public void SelectCreature(Creature target) {
            if (!gameObject.activeSelf) return;
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Encounter {
    public class PlayerController : MonoBehaviour
    {
        private void Update() {
            if (Input.GetKeyDown(KeyCode.Return)) {
                Manager.instance.AddEventToProcess(new Context(Action.TURN_END, Game.instance.player, Game.instance.player, 0));
                Manager.instance.ProcessEvents();
            }
        }

        private int index;
        private Creature target;

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
            this.index = 0;
        }
    }
}
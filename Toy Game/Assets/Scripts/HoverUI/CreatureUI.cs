using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoverUI {
    [System.Serializable]
    public class CreatureUI : UIInfo
    {
        // creatures arent a hoverableUI, so this is just used for the health bar

        public string name;
        private Creature owner;

        public void SetCreature(Creature owner) {
            this.owner = owner;
        }

        public override string GetName() {
            return name+" "+Mathf.Clamp(owner.health, 0, owner.maxHealth)+"/"+owner.maxHealth;
        }

        public override string GetDescription() {
            return "";
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Encounter;

namespace Items {
    public abstract class Item : ScriptableObject
    {
        public UIInfo ui;

        public int imaginationCost;
        public int healthCost;
        public bool used;

        public Priorities priorities;
        public TargetWeights targetWeights;

        public void Use(Context context, Creature owner) {
            used = true;
            OnUse(context, owner);
        }

        public void Event(Context context, Creature owner) {
            if (context.action == Action.TurnEnd) {
                used = false;
            }

            OnEvent(context, owner);
        }

        public bool CanUse(Creature owner) {
            return !used && healthCost >= 0 && imaginationCost >= 0 && healthCost <= owner.health && imaginationCost <= owner.imagination;
        }

        protected abstract void OnUse(Context context, Creature owner);

        protected abstract void OnEvent(Context context, Creature owner);

        [System.Serializable]
        public class TargetWeights {
            public float self = 1f;
            public float ally = 1f;
            public float enemy = 1f;
        }
    }
}
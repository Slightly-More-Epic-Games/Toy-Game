using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Encounter;
using HoverUI;

namespace Items {
    public abstract class Item : ScriptableObject
    {
        public ItemUI ui;

        public int imaginationCost;
        public int healthCost;

        public Priorities priorities;
        public TargetWeights targetWeights;

        public Refresh refresh = Refresh.TurnEnd;

        // Event and Use look similar, but Use will always be an ItemUsed action and event will be anything else
        // context is still needed to be given to Use since it has the target as well
        public void Use(Context context, Creature owner) {
            OnUse(context, owner);
        }

        public void Event(Context context, Creature owner) {
            OnEvent(context, owner);
        }

        public bool CanUse(Creature owner) {
            // you can use an item if it would leave you on 0 imagination, but not if it would leave you on 0 health
            return healthCost >= 0 && imaginationCost >= 0 && healthCost < owner.health && imaginationCost <= owner.imagination;
        }

        protected abstract void OnUse(Context context, Creature owner);

        protected abstract void OnEvent(Context context, Creature owner);

        // target weights are used to bias the AI into picking the correct target for the item
        // since otherawise theres nothing stopping them from using sword on an ally
        [System.Serializable]
        public class TargetWeights {
            public float self = 1f;
            public float ally = 1f;
            public float enemy = 1f;
        }

        // different items refresh at different points in an encounter
        // most items can only be used once per turn
        // this is all tracked in ItemSlot.cs - since multiple instances of the same item can exist
        [System.Serializable]
        public enum Refresh {
            EventFinished,
            TurnEnd,
            EncounterEnd,
            Never
        }
    }
}
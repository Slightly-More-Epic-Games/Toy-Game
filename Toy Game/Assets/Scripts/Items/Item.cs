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

        public void Use(Context context, Creature owner) {
            OnUse(context, owner);
        }

        public void Event(Context context, Creature owner) {
            OnEvent(context, owner);
        }

        public bool CanUse(Creature owner) {
            return healthCost >= 0 && imaginationCost >= 0 && healthCost < owner.health && imaginationCost <= owner.imagination;
        }

        protected abstract void OnUse(Context context, Creature owner);

        protected abstract void OnEvent(Context context, Creature owner);

        [System.Serializable]
        public class TargetWeights {
            public float self = 1f;
            public float ally = 1f;
            public float enemy = 1f;
        }

        [System.Serializable]
        public enum Refresh {
            EventFinished,
            TurnEnd,
            EncounterEnd,
            Never
        }
    }
}
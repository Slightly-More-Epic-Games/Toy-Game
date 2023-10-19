using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Encounter;

namespace Items {
    public abstract class Item : ScriptableObject
    {
        public UIInfo UI;

        public int imaginationCost;
        public int healthCost;

        public abstract void OnUse(Context context, Creature owner);

        public abstract void OnEvent(Context context, Creature owner);
    }
}
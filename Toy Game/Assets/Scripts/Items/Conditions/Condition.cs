using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Encounter;

namespace Items.Conditions {
    public abstract class Condition : ScriptableObject
    {
        public abstract bool Test(Context context, Creature owner, int[] parameters);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Encounter;

namespace Items.Conditions {
    public abstract class Condition : ScriptableObject
    {
        // conditions are used to determine when to activate a trigger, provided the ConditionData is insufficient
        // this is usually because the value of the action needs checking
        public abstract bool Test(Context context, Creature owner, int[] parameters);
    }
}

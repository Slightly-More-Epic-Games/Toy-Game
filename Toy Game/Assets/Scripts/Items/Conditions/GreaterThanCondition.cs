using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Encounter;

namespace Items.Conditions {
    [CreateAssetMenu(fileName = "Greater Than Condition", menuName = "Toy Game/GreaterThanCondition", order = 0)]
    public class GreaterThanCondition : Condition
    {
        public override bool Test(Context context, Creature owner, int[] parameters) {
            if (context.value > parameters[0]) return true;
            return false;
        }
    }
}
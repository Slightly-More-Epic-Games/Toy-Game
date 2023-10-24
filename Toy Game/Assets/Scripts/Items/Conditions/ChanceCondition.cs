using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Encounter;

namespace Items.Conditions {
    [CreateAssetMenu(fileName = "Chance Condition", menuName = "Toy Game/Conditions/ChanceCondition", order = 0)]
    public class ChanceCondition : Condition
    {
        public override bool Test(Context context, Creature owner, int[] parameters) {
            if (Random.Range(0,100) < parameters[0]) return true;
            return false;
        }
    }
}
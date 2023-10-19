using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Always Condition", menuName = "Toy Game/AlwaysCondition", order = 0)]
public class AlwaysCondition : Condition
{
    public override bool Test(Context context, Creature owner, int[] parameters) {
        return true;
    }
}

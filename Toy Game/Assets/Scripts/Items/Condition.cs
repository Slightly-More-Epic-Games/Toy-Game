using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Condition : ScriptableObject
{
    public abstract bool Test(Encounter.Context context, Creature owner, int[] parameters);
}

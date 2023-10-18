using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Condition : ScriptableObject
{
    public abstract bool Test(Context context, Creature owner, Context.Target target, Context.Action action, int[] parameters);
}

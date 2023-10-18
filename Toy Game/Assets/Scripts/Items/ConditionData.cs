using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConditionData
{
    public Context.Action actionMatch;
    public Context.Target targetMatch;
    public Condition condition;
    public int[] parameters;

    public Result resultOnSuccess;

    public enum Result {
        PASS,
        END,
        ACTIVATE,
        ACTIVATE_END
    }

    public Result Test(Context context, Creature owner) {
        if (context.action != actionMatch) return Result.PASS;
        if (!context.GetTargets(targetMatch, owner).Contains(owner)) return Result.PASS;

        bool success = condition.Test(context, owner, targetMatch, actionMatch, parameters);
        return success ? Result.PASS : resultOnSuccess;
    }
}

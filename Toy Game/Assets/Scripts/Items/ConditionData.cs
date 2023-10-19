using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConditionData
{
    public Encounter.Action actionMatch;
    public OwnerMatch ownerMatch;
    public Condition condition;
    public int[] parameters;

    public Result resultOnSuccess;

    public enum OwnerMatch {
        ANY,
        SOURCE,
        TARGET,
        EITHER
    }

    public enum Result {
        PASS,
        END,
        ACTIVATE,
        ACTIVATE_END
    }

    public Result Test(Encounter.Context context, Creature owner) {
        if (context.action != actionMatch) return Result.PASS;
        if (ownerMatch == OwnerMatch.SOURCE && context.source != owner) return Result.PASS;
        if (ownerMatch == OwnerMatch.TARGET && context.target != owner) return Result.PASS;
        if (ownerMatch == OwnerMatch.EITHER && context.source != owner && context.target != owner) return Result.PASS;
        bool success = condition.Test(context, owner, parameters);
        return success ? resultOnSuccess : Result.PASS;
    }
}

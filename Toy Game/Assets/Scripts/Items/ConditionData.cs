using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Encounter;
using Items.Conditions;

namespace Items {
    // wrapper class for a Condition scriptable object to handle passing in parameters and doing some preemptive checks
    // this is just a serializable class, since it will vary greatly per trigger, and doesnt really need to be more specific such that a scriptable object would be more appropriate

    [System.Serializable]
    public class ConditionData
    {
        public Action actionMatch = Action.AnyItemUsed;
        public OwnerMatch ownerMatch;
        public Condition condition;
        public int[] parameters;

        public Result resultOnSuccess;

        public enum OwnerMatch {
            Any,
            Source,
            Target,
            Either
        }

        public enum Result {
            Pass,
            End,
            Activate,
            ActivateEnd
        }

        public Result Test(Context context, Creature owner) {
            if (context.action != actionMatch) return Result.Pass;
            if (ownerMatch == OwnerMatch.Source && context.source != owner) return Result.Pass;
            if (ownerMatch == OwnerMatch.Target && context.target != owner) return Result.Pass;
            if (ownerMatch == OwnerMatch.Either && context.source != owner && context.target != owner) return Result.Pass;
            bool success = condition == null || condition.Test(context, owner, parameters);
            return success ? resultOnSuccess : Result.Pass;
        }
    }
}
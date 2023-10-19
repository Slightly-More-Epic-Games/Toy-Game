using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Trigger : ScriptableObject
{
    public Context.Action activateOn;
    public Context.Target target;
    public List<ConditionData> conditions;
    public bool cancelEvent;

    protected bool activated = false;

    public void OnEvent(Context context, Creature owner) {
        if (activateOn != context.action) return;

        List<Creature> creatures = context.GetTargets(target, owner);
        Debug.Log(creatures.Count);
        foreach (Creature creature in creatures) {
            Debug.Log(creature);
            creature.AddTrigger(Instantiate(this));
        }
    }

    public Result RunTrigger(Context context, Creature owner) {
        Result result = new Result();

        if (activated) return result;

        foreach (ConditionData conditionData in conditions) {
            ConditionData.Result newResult = conditionData.Test(context, owner);
            if (newResult == ConditionData.Result.ACTIVATE) result.activated = true;
            else if (newResult == ConditionData.Result.END) result.ended = true;
            else if (newResult == ConditionData.Result.ACTIVATE_END) {
                result.activated = true;
                result.ended = true;
            }
        }

        if (conditions.Count == 0) {
            result.activated = true;
            result.ended = true;
        }

        activated = true;

        if (result.activated) {
            Activate(context, owner);
        }

        result.cancelled = cancelEvent;

        return result;
    }

    public void EventFinished() {
        activated = false;
    }

    protected abstract void Activate(Context context, Creature owner);

    public class Result {
        public bool activated = false;
        public bool ended = false;
        public bool cancelled = false;
    }
}

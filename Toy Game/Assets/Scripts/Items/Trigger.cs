using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Encounter;
using HoverUI;

namespace Items {
    public abstract class Trigger : ScriptableObject
    {
        public TriggerUI ui;

        public Action activateOn;
        public Target target;
        public List<ConditionData> conditions;
        public bool cancelEvent;

        protected bool activated = false;

        public void OnEvent(Context context, Creature owner) {
            if (activateOn != context.action) return;

            List<Creature> creatures = context.GetTargets(target, owner);
            foreach (Creature creature in creatures) {
                creature.AddTrigger(Instantiate(this));
            }
        }

        public Result RunTrigger(Context context, Creature owner) {
            Result result = new Result();

            if (activated) return result;

            foreach (ConditionData conditionData in conditions) {
                ConditionData.Result conditionResult = conditionData.Test(context, owner);;
                if (conditionResult == ConditionData.Result.Activate) result.activated = true;
                else if (conditionResult == ConditionData.Result.End) result.ended = true;
                else if (conditionResult == ConditionData.Result.ActivateEnd) {
                    result.activated = true;
                    result.ended = true;
                }
            }

            if (conditions.Count == 0) {
                result.activated = true;
                result.ended = true;
            }

            if (result.activated) {
                activated = true;
                result.cancelled = cancelEvent;
                Activate(context, owner);
            }

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
}
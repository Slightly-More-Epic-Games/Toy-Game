using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Encounter;
using HoverUI;

namespace Items {
    // triggers are a "status effect" that can be given to creatures
    // most are simple things like "deal 1 damage", but some last for multiple turns and only do something when conditions are met
    public abstract class Trigger : ScriptableObject
    {
        public TriggerUI ui;

        public Action activateOn;
        public Target target;
        public List<ConditionData> conditions;
        public CancelMode cancelMode;

        protected bool activated = false;

        public void OnEvent(Context context, Creature owner) {
            // each trigger activates on a particular event
            if (activateOn != context.action) return;

            // if it does activate, it gives each creature a copy of itself
            // this code is now not used by each creature - instead they call RunTrigger
            List<Creature> creatures = context.GetTargets(target, owner);
            foreach (Creature creature in creatures) {
                creature.AddTrigger(Instantiate(this));
            }
        }

        public Result RunTrigger(Context context, Creature owner) {
            Result result = new Result();

            // dont let a trigger activate again if its already activated before EventFinished()
            // this means a "convert healing event into damage" and a "convert damage event into healing" wont loop infinitely
            if (activated) {
                return result;
            }

            foreach (ConditionData conditionData in conditions) {
                // if any condition activates, the trigger is activated
                // if any condition ends, the trigger ends (gets removed from the creature its on)
                ConditionData.Result conditionResult = conditionData.Test(context, owner);;
                if (conditionResult == ConditionData.Result.Activate) result.activated = true;
                else if (conditionResult == ConditionData.Result.End) result.ended = true;
                else if (conditionResult == ConditionData.Result.ActivateEnd) {
                    result.activated = true;
                    result.ended = true;
                }
            }

            // if there are no conditions, it both activates and ends
            // since this is the most useful case (eg for a sword, where it deals damage immedietly and doesnt need a lasting status effect)
            if (conditions.Count == 0) {
                result.activated = true;
                result.ended = true;
            }

            if (result.activated) {
                activated = true;
                
                // cancelling the event means the creature processing the event wont finish processing its effects
                // it will still go through the rest of its triggers
                // this can be used to make a status effect which blocks damage
                result.cancelMode = cancelMode;

                // now its the duty of a class extending trigger to do something based on the current context and the owner of the trigger status effect
                if (cancelMode != CancelMode.Replace) {
                    owner.AddTriggerToActivate(this);
                } else {
                    Activate(context, owner);
                }
            }

            return result;
        }

        public void EventFinished() {
            AllowActivations();
        }

        public void AllowActivations() {
            activated = false;
        }

        public abstract void Activate(Context context, Creature owner);

        public class Result {
            public bool activated = false;
            public bool ended = false;
            public CancelMode cancelMode = CancelMode.None;
        }

        [System.Serializable]
        public enum CancelMode {
            None,
            Cancel,
            Replace
        }

        public static CancelMode GetDominantCancelMode(CancelMode a, CancelMode b) {
            if (a == CancelMode.Replace) return a;
            if (b == CancelMode.Replace) return b;
            if (a == CancelMode.Cancel) return a;
            if (b == CancelMode.Cancel) return b;
            return a;
        }
    }
}
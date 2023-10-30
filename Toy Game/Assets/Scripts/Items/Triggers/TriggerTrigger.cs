using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Encounter;

namespace Items.Triggers {
    [CreateAssetMenu(fileName = "TriggerTrigger", menuName = "Toy Game/Items/Trigger Trigger", order = 0)]
    public class TriggerTrigger : Trigger
    {
        public List<Trigger> triggers;

        // when a trigger trigger is activated, it gives a trigger to the relevant targets
        // this can be used for conditionally adding another status effect
        //eg when doing a chance to poision, first a trigger is given, which ends immidietly but has a chance to activate, creating a new poison trigger, which then activates every turn
        protected override void Activate(Context context, Creature owner) {
            foreach (Trigger trigger in triggers) {
                List<Creature> creatures = context.GetTargets(trigger.target, owner);
                foreach (Creature creature in creatures) {
                    creature.AddTrigger(Instantiate(trigger));
                }
            }
        }
    }
}
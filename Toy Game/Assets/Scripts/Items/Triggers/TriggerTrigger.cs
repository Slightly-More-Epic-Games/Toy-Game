using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Encounter;

namespace Items.Triggers {
    [CreateAssetMenu(fileName = "TriggerTrigger", menuName = "Toy Game/Items/Trigger Trigger", order = 0)]
    public class TriggerTrigger : Trigger
    {
        public List<Trigger> triggers;

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
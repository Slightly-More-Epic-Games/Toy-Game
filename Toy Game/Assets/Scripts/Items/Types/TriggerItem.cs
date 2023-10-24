using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Encounter;

namespace Items.Types {
    [CreateAssetMenu(fileName = "TriggerItem", menuName = "Toy Game/Items/Trigger Item", order = 0)]
    public class TriggerItem : Item
    {
        public List<Trigger> triggers;

        protected override void OnEvent(Context context, Creature owner) {
            foreach (Trigger trigger in triggers) {
                trigger.OnEvent(context, owner);
            }
        }

        protected override void OnUse(Context context, Creature owner) {
            foreach (Trigger trigger in triggers) {
                trigger.OnEvent(context, owner);
            }
        }
    }
}
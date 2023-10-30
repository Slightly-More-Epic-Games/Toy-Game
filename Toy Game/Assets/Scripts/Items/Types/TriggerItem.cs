using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Encounter;

namespace Items.Types {
    [CreateAssetMenu(fileName = "TriggerItem", menuName = "Toy Game/Items/Trigger Item", order = 0)]
    public class TriggerItem : Item
    {
        public List<Trigger> triggers;

        // a trigger item is an item that gives triggers to creatures when used, or when a particular event happens
        // if an item was needed that didnt use triggers, a new class extending item would be made
        // the game will then handle it completely fine - this same pattern of the game not caring about specific class types is true for a lot of item code
        // this means if we wanted an item that, for instance, summons a creature, it wouldnt be too hard to add

        protected override void OnEvent(Context context, Creature owner) {
            foreach (Trigger trigger in triggers) {
                trigger.OnEvent(context, owner);
            }
        }

        // unlike items, triggers dont make a distinction between using an item and any other event
        protected override void OnUse(Context context, Creature owner) {
            foreach (Trigger trigger in triggers) {
                trigger.OnEvent(context, owner);
            }
        }
    }
}
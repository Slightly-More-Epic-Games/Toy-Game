using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TriggerItem", menuName = "Toy Game/TriggerItem", order = 0)]
public class TriggerItem : Item
{
    public List<Trigger> triggers;

    public override void OnEvent(Encounter.Context context, Creature owner) {
        foreach (Trigger trigger in triggers) {
            trigger.OnEvent(context, owner);
        }
    }

    public override void OnUse(Encounter.Context context, Creature owner) {
        foreach (Trigger trigger in triggers) {
            trigger.OnEvent(context, owner);
        }
    }
}

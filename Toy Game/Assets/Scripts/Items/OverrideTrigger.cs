using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OverrideTrigger", menuName = "Toy Game/Override Trigger", order = 0)]
public class OverrideTrigger : Trigger
{
    public List<ContextOverride> overrides;

    protected override void Activate(Context context, Creature owner) {
        foreach (ContextOverride contextOverride in overrides) {
            contextOverride.Apply(context, owner);
        }
    }
}

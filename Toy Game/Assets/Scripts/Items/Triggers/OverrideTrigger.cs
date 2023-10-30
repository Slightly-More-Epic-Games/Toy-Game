using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Encounter;

namespace Items.Triggers {
    [CreateAssetMenu(fileName = "OverrideTrigger", menuName = "Toy Game/Items/Override Trigger", order = 0)]
    public class OverrideTrigger : Trigger
    {
        public List<ContextOverride> overrides;

        // an override trigger takes the current context, and then creates a new context based on the current ones values
        // this is done using ContextOverrides
        protected override void Activate(Context context, Creature owner) {
            foreach (ContextOverride contextOverride in overrides) {
                contextOverride.Apply(context, owner);
            }
        }
    }
}
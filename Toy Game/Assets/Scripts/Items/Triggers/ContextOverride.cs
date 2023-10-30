using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NCalc;
using Encounter;

namespace Items.Triggers {
    [System.Serializable]
    public class ContextOverride
    {
        public Action action;
        public Target source;
        public Target target;
        public string valueOverride;

        private Expression expression = null;

        public void Apply(Context context, Creature owner) {
            // the new value can be the same as before "", a constant "3", or a calculation from before "x*2"
            // the calculation is done using NCalc
            int newValue;
            if (valueOverride == "" || valueOverride == "x") {
                newValue = context.value;
            } else {
                expression ??= new Expression(valueOverride.Replace("x", "[X]"));
                expression.Parameters["X"] = context.value;
                float result = float.Parse(expression.Evaluate().ToString(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                newValue = Mathf.CeilToInt(result);
            }

            // the new sources and targets can be a range of things, eg the same, swapped - most (eg all enemies) are in relation to the creature the overridetrigger is attached to
            List<Creature> sources = context.GetTargets(source, owner);
            List<Creature> targets = context.GetTargets(target, owner);

            // create the new event and add it to the list of events to process in the current processingness
            // each source targets each target with the new action and new value
            foreach (Creature sourceCreature in sources) {
                foreach (Creature targetCreature in targets) {
                    Manager.instance.AddEventToProcess(new Context(action, sourceCreature, targetCreature, newValue));
                }
            }
        }
    }
}
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
            int newValue;
            if (valueOverride == "" || valueOverride == "x") {
                newValue = context.value;
            } else {
                expression ??= new Expression(valueOverride.Replace("x", "[X]"));
                expression.Parameters["X"] = context.value;
                float result = float.Parse(expression.Evaluate().ToString(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                newValue = Mathf.CeilToInt(result);
            }

            List<Creature> sources = context.GetTargets(source, owner);
            List<Creature> targets = context.GetTargets(target, owner);
            foreach (Creature sourceCreature in sources) {
                foreach (Creature targetCreature in targets) {
                    Manager.instance.AddEventToProcess(new Context(action, sourceCreature, targetCreature, newValue));
                }
            }
        }
    }
}
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
                newValue = Mathf.RoundToInt(result);
            }

            Debug.Log("making new context "+action+" from context "+context.action+" "+context.source+" "+context.target);
            Context newContext = new Context(action, context.GetTargets(source, owner)[0], context.GetTargets(target, owner)[0], newValue);
            Debug.Log("new context: "+newContext.source+" "+newContext.target);

            Manager.instance.AddEventToProcess(newContext);
        }
    }
}
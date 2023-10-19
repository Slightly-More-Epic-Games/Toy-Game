using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NCalc;

[System.Serializable]
public class ContextOverride
{
    public Context.Action action;
    public Context.Target source;
    public Context.Target target;
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

        Context newContext = new Context(action, context.GetTargets(source, owner)[0], context.GetTargets(target, owner)[0], newValue);

        Game.instance.AddEventToProcess(newContext);
    }
}

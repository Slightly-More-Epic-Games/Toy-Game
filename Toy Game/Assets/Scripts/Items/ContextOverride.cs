using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ContextOverride
{
    public Context.Action action;
    public Context.Target source;
    public Context.Target target;
    public string valueOverride;

    public void Apply(Context context, Creature owner) {
        Context newContext = new Context(action, context.GetTargets(source, owner)[0], context.GetTargets(target, owner)[0], context.value);

        Game.instance.AddEventToProcess(newContext);
    }
}

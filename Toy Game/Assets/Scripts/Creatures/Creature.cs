using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Creature : ScriptableObject {
    public int health;
    public int imagination;
    public List<Item> items;

    protected List<Trigger> triggers;

    public void AddTrigger(Trigger trigger) {
        triggers.Add(trigger);
    }

    public void UseItem(int index, Creature target) {
        Item item = items[index];
        item.OnUse(new Context(Context.Action.ITEM_USED, this, target, index), this);
        Game.instance.AddEventToProcess(new Context(Context.Action.ANY_ITEM_USED, this, target, index));
        Game.instance.ProcessEvents();
    }

    public void OnEvent(Context context) {
        foreach (Item item in items) {
            item.OnEvent(context, this);
        }

        bool cancelled = false;
        for (int i = 0; i < triggers.Count; i++) {
            Trigger.Result result = triggers[i].RunTrigger(context, this);
            if (result.ended) {
                triggers.RemoveAt(i);
                i--;
            }
            cancelled |= result.cancelled;
        }

        if (!cancelled) {
            ProcessEvent(context);
        }
    }

    protected void ProcessEvent(Context context) {
        Debug.Log("creature "+name+" is processing event");
        Debug.Log(context.action);
        Debug.Log(context.source);
        Debug.Log(context.target);
        Debug.Log(context.value);
        Debug.Log("-----------");
    }
}

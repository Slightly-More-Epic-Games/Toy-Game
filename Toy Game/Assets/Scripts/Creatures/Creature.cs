using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Creature : ScriptableObject {
    public int health;
    public int imagination;
    public List<Item> items;

    protected List<Trigger> triggers = new List<Trigger>();

    public void AddTrigger(Trigger trigger) {
        triggers.Add(trigger);
    }

    public void UseItem(int index, Creature target) {
        Item item = items[index];
        Game.instance.AddEventToProcess(new Context(Context.Action.ANY_ITEM_USED, this, target, index));

        if (item.imaginationCost > 0) Game.instance.AddEventToProcess(new Context(Context.Action.LOSE_IMAGINATION, this, this, item.imaginationCost));
        if (item.healthCost > 0) Game.instance.AddEventToProcess(new Context(Context.Action.LOSE_HEALTH, this, this, item.healthCost));

        item.OnUse(new Context(Context.Action.ITEM_USED, this, target, index), this);
        Game.instance.ProcessEvents();
    }

    public void OnEvent(Context context) {
        foreach (Item item in items) {
            item.OnEvent(context, this);
        }

        bool cancelled = false;
        for (int i = 0; i < triggers.Count; i++) {
            Trigger trigger = triggers[i];
            Trigger.Result result = trigger.RunTrigger(context, this);
            if (result.ended) {
                triggers.RemoveAt(i);
                Destroy(trigger);
                i--;
            }
            cancelled |= result.cancelled;
        }

        if (!cancelled) {
            ProcessEvent(context);
        }
    }

    public void EventFinished() {
        foreach (Trigger trigger in triggers) {
            trigger.EventFinished();
        }
    }

    protected void ProcessEvent(Context context) {
        Debug.Log("creature "+name+" is processing event");
        Debug.Log(context.action);
        Debug.Log(context.source);
        Debug.Log(context.target);
        Debug.Log(context.value);
        Debug.Log("-----------");

        switch (context.action) {
            case Context.Action.ITEM_USED:
            case Context.Action.ANY_ITEM_USED:
                break;
            case Context.Action.DEAL_DAMAGE:
                if (context.source != this) return;
                Game.instance.AddEventToProcess(new Context(Context.Action.LOSE_HEALTH, context.source, context.target, context.value));
                break;
            case Context.Action.GAIN_HEALTH:
            case Context.Action.LOSE_HEALTH:
                if (context.target != this) return;
                HealthChange(context.action == Context.Action.GAIN_HEALTH ? context.value : -context.value);
                break;
            case Context.Action.GAIN_IMAGINATION:
            case Context.Action.LOSE_IMAGINATION:
                if (context.target != this) return;
                ImaginationChange(context.action == Context.Action.GAIN_IMAGINATION ? context.value : -context.value);
                break;
            case Context.Action.TURN_START:
            case Context.Action.TURN_END:
            case Context.Action.ENCOUNTER_START:
            case Context.Action.ENCOUNTER_END:
                break;
        }

        Debug.Log("new stats: health:"+health+" imagination:"+imagination);
    }

    protected void HealthChange(int delta) {
        health += delta;
    }

    protected void ImaginationChange(int delta) {
        imagination += delta;
    }
}

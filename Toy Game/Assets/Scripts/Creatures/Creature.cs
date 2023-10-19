using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Creature : ScriptableObject {
    public int health;
    public int imagination;
    public List<Item> items;

    protected List<Trigger> triggers = new List<Trigger>();

    public Creature lastAttacker;

    protected bool isDead;

    public void AddTrigger(Trigger trigger) {
        triggers.Add(trigger);
    }

    public void UseItem(int index, Creature target) {
        Item item = items[index];
        Encounter.instance.AddEventToProcess(new Context(Context.Action.ANY_ITEM_USED, this, target, index));

        if (item.imaginationCost > 0) Encounter.instance.AddEventToProcess(new Context(Context.Action.LOSE_IMAGINATION, this, this, item.imaginationCost));
        if (item.healthCost > 0) Encounter.instance.AddEventToProcess(new Context(Context.Action.LOSE_HEALTH, this, this, item.healthCost));

        item.OnUse(new Context(Context.Action.ITEM_USED, this, target, index), this);
        Encounter.instance.ProcessEvents();
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
        switch (context.action) {
            case Context.Action.ITEM_USED:
            case Context.Action.ANY_ITEM_USED:
                break;
            case Context.Action.DEAL_DAMAGE:
                if (context.source != this) return;
                Encounter.instance.AddEventToProcess(new Context(Context.Action.LOSE_HEALTH, context.source, context.target, context.value));
                break;
            case Context.Action.GAIN_HEALTH:
            case Context.Action.LOSE_HEALTH:
                if (context.target != this) return;
                HealthChange(context.action == Context.Action.GAIN_HEALTH ? context.value : -context.value, context.source);
                break;
            case Context.Action.GAIN_IMAGINATION:
            case Context.Action.LOSE_IMAGINATION:
                if (context.target != this) return;
                ImaginationChange(context.action == Context.Action.GAIN_IMAGINATION ? context.value : -context.value);
                break;
            case Context.Action.TURN_START:
                if (context.target != this) return;
                OnTurnStart();
                break;
            case Context.Action.TURN_END:
                if (context.source != this) return;
                OnTurnEnd();
                break;
            case Context.Action.ENCOUNTER_START:
                OnEncounterStart();
                break;
            case Context.Action.ENCOUNTER_END:
                OnEncounterEnd();
                break;
        }

        Debug.Log("new stats for "+name+": health:"+health+" imagination:"+imagination);
    }

    protected void HealthChange(int delta, Creature source) {
        health += delta;
        if (delta < 0) lastAttacker = source;
    }

    protected void ImaginationChange(int delta) {
        imagination += delta;
    }

    protected virtual void OnTurnStart() {}

    protected virtual void OnTurnEnd() {}

    protected virtual void OnEncounterStart() {}

    protected virtual void OnEncounterEnd() {}

    protected void EndTurn() {
        Encounter.instance.AddEventToProcess(new Context(Context.Action.TURN_END, this, this, 0));
        Encounter.instance.ProcessEvents();
    }

    public bool UpdateDeadness() {
        if (isDead) return true;
        if (health > 0) return false;
        OnEvent(new Context(Context.Action.LAST_STAND, this, this, health));
        if (health > 0) return false;
        Encounter.instance.AddEventToProcess(new Context(Context.Action.ANY_DEATH, this, this, health));
        OnDeath();
        isDead = true;
        Destroy(this);
        return true;
    }

    protected virtual void OnDeath() {}
}

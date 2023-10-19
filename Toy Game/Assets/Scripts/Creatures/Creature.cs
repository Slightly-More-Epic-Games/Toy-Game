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
        Encounter.Manager.instance.AddEventToProcess(new Encounter.Context(Encounter.Action.ANY_ITEM_USED, this, target, index));

        if (item.imaginationCost > 0) Encounter.Manager.instance.AddEventToProcess(new Encounter.Context(Encounter.Action.LOSE_IMAGINATION, this, this, item.imaginationCost));
        if (item.healthCost > 0) Encounter.Manager.instance.AddEventToProcess(new Encounter.Context(Encounter.Action.LOSE_HEALTH, this, this, item.healthCost));

        item.OnUse(new Encounter.Context(Encounter.Action.ITEM_USED, this, target, index), this);
        Encounter.Manager.instance.ProcessEvents();
    }

    public void OnEvent(Encounter.Context context) {
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

    protected void ProcessEvent(Encounter.Context context) {
        switch (context.action) {
            case Encounter.Action.ITEM_USED:
            case Encounter.Action.ANY_ITEM_USED:
                break;
            case Encounter.Action.DEAL_DAMAGE:
                if (context.source != this) return;
                Encounter.Manager.instance.AddEventToProcess(new Encounter.Context(Encounter.Action.LOSE_HEALTH, context.source, context.target, context.value));
                break;
            case Encounter.Action.GAIN_HEALTH:
            case Encounter.Action.LOSE_HEALTH:
                if (context.target != this) return;
                HealthChange(context.action == Encounter.Action.GAIN_HEALTH ? context.value : -context.value, context.source);
                break;
            case Encounter.Action.GAIN_IMAGINATION:
            case Encounter.Action.LOSE_IMAGINATION:
                if (context.target != this) return;
                ImaginationChange(context.action == Encounter.Action.GAIN_IMAGINATION ? context.value : -context.value);
                break;
            case Encounter.Action.TURN_START:
                if (context.target != this) return;
                OnTurnStart();
                break;
            case Encounter.Action.TURN_END:
                if (context.source != this) return;
                OnTurnEnd();
                break;
            case Encounter.Action.ENCOUNTER_START:
                OnEncounterStart();
                break;
            case Encounter.Action.ENCOUNTER_END:
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
        Encounter.Manager.instance.AddEventToProcess(new Encounter.Context(Encounter.Action.TURN_END, this, this, 0));
        Encounter.Manager.instance.ProcessEvents();
    }

    public bool UpdateDeadness() {
        if (isDead) return true;
        if (health > 0) return false;
        OnEvent(new Encounter.Context(Encounter.Action.LAST_STAND, this, this, health));
        if (health > 0) return false;
        Encounter.Manager.instance.AddEventToProcess(new Encounter.Context(Encounter.Action.ANY_DEATH, this, this, health));
        OnDeath();
        isDead = true;
        Destroy(this);
        return true;
    }

    protected virtual void OnDeath() {}
}

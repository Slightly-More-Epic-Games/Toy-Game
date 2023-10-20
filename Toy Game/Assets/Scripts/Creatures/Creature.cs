using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Items;
using Encounter;

public abstract class Creature : ScriptableObject {
    public int health;
    public int imagination;
    public List<Item> items;

    [System.NonSerialized] public List<Trigger> triggers = new List<Trigger>();

    [System.NonSerialized] public Creature lastAttacker;

    protected bool isDead;

    [System.NonSerialized] public CreatureVisual creatureVisual;

    public void AddTrigger(Trigger trigger) {
        triggers.Add(trigger);
    }

    public void UseItem(int index, Creature target) {
        Item item = items[index];
        Manager.instance.AddEventToProcess(new Context(Action.ANY_ITEM_USED, this, target, index));

        if (item.imaginationCost > 0) Manager.instance.AddEventToProcess(new Context(Action.LOSE_IMAGINATION, this, this, item.imaginationCost));
        if (item.healthCost > 0) Manager.instance.AddEventToProcess(new Context(Action.LOSE_HEALTH, this, this, item.healthCost));

        item.OnUse(new Context(Action.ITEM_USED, this, target, index), this);
        Manager.instance.ProcessEvents();
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

    public virtual void EventFinished() {
        foreach (Trigger trigger in triggers) {
            trigger.EventFinished();
        }

        creatureVisual.UpdateVisual(this);
    }

    protected void ProcessEvent(Context context) {
        switch (context.action) {
            case Action.ITEM_USED:
            case Action.ANY_ITEM_USED:
                break;
            case Action.DEAL_DAMAGE:
                if (context.source != this) return;
                Debug.Log(name+" dealing damage to "+context.target);
                Manager.instance.AddEventToProcess(new Context(Action.LOSE_HEALTH, context.source, context.target, context.value));
                break;
            case Action.GAIN_HEALTH:
            case Action.LOSE_HEALTH:
                if (context.target != this) return;
                Debug.Log(name+" taking damage from "+context.source);
                HealthChange(context.action == Action.GAIN_HEALTH ? context.value : -context.value, context.source);
                break;
            case Action.GAIN_IMAGINATION:
            case Action.LOSE_IMAGINATION:
                if (context.target != this) return;
                ImaginationChange(context.action == Action.GAIN_IMAGINATION ? context.value : -context.value);
                break;
            case Action.TURN_START:
                if (context.target != this) return;
                OnTurnStart();
                break;
            case Action.TURN_END:
                if (context.source != this) return;
                OnTurnEnd();
                break;
            case Action.ENCOUNTER_START:
                OnEncounterStart();
                break;
            case Action.ENCOUNTER_END:
                OnEncounterEnd();
                break;
        }
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
        if (Manager.instance.currentTurn != this) return;
        Manager.instance.AddEventToProcess(new Context(Action.TURN_END, this, this, 0));
        Manager.instance.ProcessEvents();
    }

    public bool UpdateDeadness() {
        if (isDead) return true;
        if (health > 0) return false;
        OnEvent(new Context(Action.LAST_STAND, this, this, health));
        if (health > 0) return false;
        Manager.instance.AddEventToProcess(new Context(Action.ANY_DEATH, this, this, health));
        OnDeath();
        isDead = true;
        Destroy(this);
        return true;
    }

    protected virtual void OnDeath() {}
}

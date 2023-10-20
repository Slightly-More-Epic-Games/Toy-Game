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

    public virtual void UpdateTurn() {}

    public void AddTrigger(Trigger trigger) {
        triggers.Add(trigger);
    }

    public void UseItem(int index, Creature target) {
        Item item = items[index];
        Manager.instance.AddEventToProcess(new Context(Action.AnyItemUsed, this, target, index));

        if (item.imaginationCost > 0) Manager.instance.AddEventToProcess(new Context(Action.LoseImagination, this, this, item.imaginationCost));
        if (item.healthCost > 0) Manager.instance.AddEventToProcess(new Context(Action.LoseHealth, this, this, item.healthCost));

        item.OnUse(new Context(Action.ItemUsed, this, target, index), this);
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
            case Action.ItemUsed:
            case Action.AnyItemUsed:
                break;
            case Action.DealDamage:
                if (context.source != this) return;
                Manager.instance.AddEventToProcess(new Context(Action.LoseHealth, context.source, context.target, context.value));
                break;
            case Action.GainHealth:
            case Action.LoseHealth:
                if (context.target != this) return;
                HealthChange(context.action == Action.GainHealth ? context.value : -context.value, context.source);
                break;
            case Action.GainImagination:
            case Action.LoseImagination:
                if (context.target != this) return;
                ImaginationChange(context.action == Action.GainImagination ? context.value : -context.value);
                break;
            case Action.TurnStart:
                if (context.target != this) return;
                OnTurnStart();
                break;
            case Action.TurnEnd:
                if (context.source != this) return;
                OnTurnEnd();
                break;
            case Action.EncounterStart:
                OnEncounterStart();
                break;
            case Action.EncounterEnd:
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
        Manager.instance.AddEventToProcess(new Context(Action.TurnEnd, this, this, 0));
        Manager.instance.ProcessEvents();
    }

    public bool UpdateDeadness() {
        if (isDead) return true;
        if (health > 0) return false;
        OnEvent(new Context(Action.LastStand, this, this, health));
        if (health > 0) return false;
        Manager.instance.AddEventToProcess(new Context(Action.AnyDeath, this, this, health));
        OnDeath();
        isDead = true;
        Destroy(this);
        return true;
    }

    protected virtual void OnDeath() {}
}

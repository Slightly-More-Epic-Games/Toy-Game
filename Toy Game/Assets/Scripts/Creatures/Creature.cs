using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Items;
using Encounter;
using HoverUI;

public abstract class Creature : ScriptableObject {
    public Sprite[] sprites;
    public float fps;

    public CreatureUI ui;

    public int maxHealth;
    public int maxImagination;
    [System.NonSerialized] public int health;
    [System.NonSerialized] public int imagination;
    [SerializeField] protected List<Item> inventory;
    [System.NonSerialized] public List<ItemSlot> items;

    [System.NonSerialized] public List<Trigger> triggers = new List<Trigger>();

    [System.NonSerialized] public Creature lastAttacker;

    [System.NonSerialized] public bool isDead;

    [System.NonSerialized] public CreatureVisual creatureVisual;

    [System.NonSerialized] public CreatureController controller;

    public float spawnCost = 1f;

    public Priorities priorities;

    public List<Item> dropPool;

    public void Initialise() {
        health = maxHealth;
        imagination = 0;
        items = new List<ItemSlot>();
        foreach (Item item in inventory) {
            AddItem(item);
        }
        ui.SetCreature(this);
    }

    public void AddItem(Item item) {
        items.Add(new ItemSlot(Instantiate(item)));
    }

    public void AddTrigger(Trigger trigger) {
        triggers.Add(trigger);
    }

    public void UseItem(int index, Creature target) {
        ItemSlot item = items[index];
        item.Use(this, target, index);
    }

    public void OnEvent(Context context) {
        foreach (ItemSlot item in items) {
            item.Event(context, this);
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

        if (health > maxHealth) {
            health = maxHealth;
        }
        if (imagination > maxImagination) {
            imagination = maxImagination;
        }

        foreach (ItemSlot item in items) {
            item.EventFinished(this);
        }

        creatureVisual.UpdateVisual(this);
        controller.OnEventsFinished(this);
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
                controller.OnTurnStart(this);
                break;
            case Action.TurnEnd:
                if (context.source != this) return;
                controller.OnTurnEnd(this);
                break;
            case Action.EncounterStart:
                controller.OnEncounterStart(this);
                break;
            case Action.EncounterEnd:
                controller.OnEncounterEnd(this);
                break;
        }
    }

    protected void HealthChange(int delta, Creature source) {
        health += delta;
        if (delta < 0 && source != this) lastAttacker = source;
    }

    protected void ImaginationChange(int delta) {
        imagination += delta;
    }

    public void EndTurn() {
        if (Manager.instance.currentTurn != this) return;
        Manager.instance.AddEventToProcess(new Context(Action.TurnEnd, this, this, 0));
        Manager.instance.ProcessEvents();
    }

    public bool UpdateDeadness() {
        if (isDead) return true;
        if (health > 0) return false;
        OnEvent(new Context(Action.LastStand, this, this, health));
        if (health > 0) return false;
        Manager.instance.CreatureDied(this, lastAttacker);
        OnDeath();
        isDead = true;
        Destroy(this);
        Destroy(creatureVisual.gameObject);
        return true;
    }

    protected virtual void OnDeath() {}
}

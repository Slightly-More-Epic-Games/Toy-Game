using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Items;
using Encounter;
using HoverUI;

public abstract class Creature : ScriptableObject {
    public SpriteAnimation spriteAnimation;

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

    public bool isLarge;

    private List<Trigger> triggersToActivate = new List<Trigger>();
    private List<Trigger> triggersToRemove = new List<Trigger>();

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
        // when using an item, first take the cost of the item
        item.PrepareToUse(this, target, index);
        // then get the controller to create the little item throw animation
        controller.UseItemPhysically(item, this, target, index);
    }

    public void OnEvent(Context context) {
        // when an event happens, first go through all items, since some items may create a trigger when something happens
        // (eg the Teddy Bear creates a "gain imagination" trigger)
        foreach (ItemSlot item in items) {
            item.Event(context, this);
        }

        // then go through all triggers
        // some of these triggers will either dissapear, or create more triggers
        // this means a foreach (Trigger trigger in triggers) {} cant be used

        triggersToActivate.Clear();
        triggersToRemove.Clear();

        Trigger.CancelMode cancelMode = Trigger.CancelMode.None;
        for (int i = 0; i < triggers.Count; i++) {
            Trigger trigger = triggers[i];
            Trigger.Result result = trigger.RunTrigger(context, this);
            // if the trigger has ended, remove it
            if (result.ended) {
                triggersToRemove.Add(trigger);
            }

            cancelMode = Trigger.GetDominantCancelMode(cancelMode, result.cancelMode);
            if (cancelMode == Trigger.CancelMode.Replace) {
                break;
            }
        }

        // some triggers (eg glue) want to replace the context with a new version
        // if this was done by adding another event normally, then items like umbrellas would block the first cancelled event, and not the second
        // triggers with such a flag marked do activate regardless of any other cancelAlls however
        if (cancelMode != Trigger.CancelMode.Replace) {
            foreach (Trigger trigger in triggersToRemove) {
                triggers.Remove(trigger);
            }
        }

        foreach (Trigger trigger in triggersToActivate) {
            if (cancelMode == Trigger.CancelMode.Replace) {
                trigger.AllowActivations();
            } else {
                trigger.Activate(context, this);
            }
        }

        // if no triggers caused the event to get cancelled, process it
        if (cancelMode == Trigger.CancelMode.None) {
            ProcessEvent(context);
        }
    }

    public void AddTriggerToActivate(Trigger trigger) {
        triggersToActivate.Add(trigger);
    }

    public virtual void EventFinished() {
        // after the encounter manager has finished processing a chain of events, this is called
        // nothing in this method should ever call ProcessEvents, for fear of an infinite loop
        foreach (Trigger trigger in triggers) {
            // this is when triggers are allowed to activate again
            trigger.EventFinished();
        }

        // limit health and imaginaton
        if (health > maxHealth) {
            health = maxHealth;
        }
        if (imagination > maxImagination) {
            imagination = maxImagination;
        }

        foreach (ItemSlot item in items) {
            item.EventFinished(this);
        }

        // update health bar and controller
        creatureVisual.UpdateVisual(this);
        controller.OnEventsFinished(this);
    }

    protected void ProcessEvent(Context context) {
        // provided an event hasnt been cancelled, its now time to process it!
        // this is just a massive switch statement that calls other methods
        // nothing needs explaining
        switch (context.action) {
            case Action.ItemUsed:
            case Action.AnyItemUsed:
                break;
            case Action.DealDamage:
                if (context.source != this) return;
                // other than this, the deal damage event creates a lose health event
                // this distinction is useful for some items (eg the hand mirror)
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
        // last stand can be used to heal a creature when it dies, preventing death
        OnEvent(new Context(Action.LastStand, this, this, health));
        if (health > 0) return false;
        // if nothing heals a creature on last stand, an AnyDeath event is made, which cant be stopped
        Manager.instance.CreatureDied(this, lastAttacker);
        // most of this code is just making sure everything gets cleared
        OnDeath();
        isDead = true;
        Destroy(this);
        Destroy(creatureVisual.gameObject);
        return true;
    }

    protected virtual void OnDeath() {}
}

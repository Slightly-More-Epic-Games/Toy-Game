using System.Collections;
using System.Collections.Generic;
using Encounter;
using Items;
using UnityEngine;
using HoverUI;

public class ItemSlot {
    // item cant be referenced directly from a class that has a reference to an itemslot
    // this helps keep code from misusing any item methods
    protected Item item;

    public bool used = false;

    // items arent really instanced properly
    // this means that to have something being tracked per creature, an itemSlot is used

    public ItemSlot(Item item) {
        this.item = item;
        item.ui.SetItem(item);
    }

    public ItemSlot Copy() {
        return new ItemSlot(Object.Instantiate(item));
    }

    public void PrepareToUse(Creature owner, Creature target, int index) {
        // PrepareToUse is called the moment an item is used - then once the little item visual hits the target, Use is called
        // by having the cost of the item processed before it hits the target, it makes it easy to follow the effect of the item
        used = true;
        
        if (item.imaginationCost > 0) Manager.instance.AddEventToProcess(new Context(Action.LoseImagination, owner, owner, item.imaginationCost));
        if (item.healthCost > 0) Manager.instance.AddEventToProcess(new Context(Action.LoseHealth, owner, owner, item.healthCost));
        Manager.instance.ProcessEvents();
    }

    public void Use(Creature owner, Creature target, int index) {
        // the AnyItemUsed event is added before the item is used, since the item will probably lead to a bunch more events happening
        Manager.instance.AddEventToProcess(new Context(Action.AnyItemUsed, owner, target, index));
        item.Use(new Context(Action.ItemUsed, owner, target, index), owner);
        Manager.instance.ProcessEvents();
    }

    public void Event(Context context, Creature owner) {
        if ((item.refresh == Item.Refresh.TurnEnd && context.action == Action.TurnEnd) || (item.refresh == Item.Refresh.EncounterEnd && context.action == Action.EncounterEnd)) {
            used = false;
        }
        item.Event(context, owner);
    }

    public void EventFinished(Creature owner) {
        if (item.refresh == Item.Refresh.EventFinished) {
            used = false;
        }
    }

    public bool CanUse(Creature owner) {
        return !used && item.CanUse(owner);
    }

    public bool IsPassive() {
        // a passive item is marked by having -1 for both costs
        // this means it cant be used, but its icon will still be light
        return item.healthCost == -1 && item.imaginationCost == -1;
    }

    public Priorities GetPriorities() {
        return item.priorities;
    }

    public Item.TargetWeights GetTargetWeights() {
        return item.targetWeights;
    }

    public ItemUI GetItemUI() {
        return item.ui;
    }
}

using System.Collections;
using System.Collections.Generic;
using Encounter;
using Items;
using UnityEngine;
using HoverUI;

public class ItemSlot {
    protected Item item;

    public bool used = false;

    public ItemSlot(Item item) {
        this.item = item;
        item.ui.SetItem(item);
    }

    public ItemSlot Copy() {
        return new ItemSlot(Object.Instantiate(item));
    }

    public void PrepareToUse(Creature owner, Creature target, int index) {
        used = true;
        
        if (item.imaginationCost > 0) Manager.instance.AddEventToProcess(new Context(Action.LoseImagination, owner, owner, item.imaginationCost));
        if (item.healthCost > 0) Manager.instance.AddEventToProcess(new Context(Action.LoseHealth, owner, owner, item.healthCost));
        Manager.instance.ProcessEvents();
    }

    public void Use(Creature owner, Creature target, int index) {
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

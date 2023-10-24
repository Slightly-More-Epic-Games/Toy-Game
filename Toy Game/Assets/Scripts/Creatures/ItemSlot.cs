using System.Collections;
using System.Collections.Generic;
using Encounter;
using Items;
using UnityEngine;

public class ItemSlot {
    protected Item item;

    public bool used = false;

    public ItemSlot(Item item) {
        this.item = item;
    }

    public void Use(Creature owner, Creature target, int index) {
        used = true;

        if (item.imaginationCost > 0) Manager.instance.AddEventToProcess(new Context(Action.LoseImagination, owner, owner, item.imaginationCost));
        if (item.healthCost > 0) Manager.instance.AddEventToProcess(new Context(Action.LoseHealth, owner, owner, item.healthCost));
        Manager.instance.ProcessEvents();

        Manager.instance.AddEventToProcess(new Context(Action.AnyItemUsed, owner, target, index));
        item.Use(new Context(Action.ItemUsed, owner, target, index), owner);
        Manager.instance.ProcessEvents();
    }

    public void Event(Context context, Creature owner) {
        if (context.action == Action.TurnEnd) {
            used = false;
        }
        item.Event(context, owner);
    }

    public bool CanUse(Creature owner) {
        return !used && item.CanUse(owner);
    }

    public Priorities GetPriorities() {
        return item.priorities;
    }

    public Item.TargetWeights GetTargetWeights() {
        return item.targetWeights;
    }

    public UIInfo GetUIInfo() {
        return item.ui;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : ScriptableObject
{
    public string itemName;
    public string itemDescription;

    public int imaginationCost;
    public int healthCost;

    public abstract void OnUse(Context context, Creature owner);

    public abstract void OnEvent(Context context, Creature owner);
}

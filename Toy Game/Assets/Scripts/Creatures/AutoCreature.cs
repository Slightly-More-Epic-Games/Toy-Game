using System.Collections;
using System.Collections.Generic;
using Encounter;
using UnityEngine;

[CreateAssetMenu(fileName = "AutoCreature", menuName = "Toy Game/Auto Creature", order = 0)]
public class AutoCreature : Creature
{
    bool active;

    protected override void OnTurnStart() {
        active = true;
    }

    protected override void OnTurnEnd() {
        active = false;
    }

    public void RunTurn() {
        if (!active) return;

        UseBestItem();
        EndTurn();
    }

    private void UseBestItem() {
        UseItem(1, Game.instance.player);
    }
}

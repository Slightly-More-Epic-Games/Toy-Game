using System.Collections;
using System.Collections.Generic;
using Encounter;
using UnityEngine;

[CreateAssetMenu(fileName = "AutoCreature", menuName = "Toy Game/Auto Creature", order = 0)]
public class AutoCreature : Creature
{
    public override void UpdateTurn() {
        UseBestItem();
        EndTurn();
    }

    private void UseBestItem() {
        UseItem(1, Game.instance.player);
    }
}

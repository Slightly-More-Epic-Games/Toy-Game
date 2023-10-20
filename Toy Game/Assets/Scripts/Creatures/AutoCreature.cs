using System.Collections;
using System.Collections.Generic;
using Encounter;
using UnityEngine;

[CreateAssetMenu(fileName = "AutoCreature", menuName = "Toy Game/Auto Creature", order = 0)]
public class AutoCreature : Creature
{
    protected override void OnTurnStart() {
        base.OnTurnStart();
        UseItem(0, Game.instance.player);
        EndTurn();
    }

    protected override void OnTurnEnd() {
        base.OnTurnEnd();
    }
}

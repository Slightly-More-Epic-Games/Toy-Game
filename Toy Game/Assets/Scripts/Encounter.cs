using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Encounter : MonoBehaviour
{
    public void PlayerUseItemTest(int a) {
        Game.instance.playerAllies[0].UseItem(a, Game.instance.playerEnemies[0]);
    }

    public void CreatureUseItemTest(int a) {
        Game.instance.playerEnemies[0].UseItem(a, Game.instance.playerAllies[0]);
    }
}

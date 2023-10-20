using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Encounter {
    public class EnemyController : CreatureController
    {
        public override void UpdateTurn(Creature owner) {
            UseBestItem(owner);
            owner.EndTurn();
        }

        private void UseBestItem(Creature owner) {
            owner.UseItem(1, Game.instance.player);
        }
    }
}
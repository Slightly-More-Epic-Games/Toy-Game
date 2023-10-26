using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map {
    [CreateAssetMenu(fileName = "EnemyPoolNode", menuName = "Toy Game/Map/EnemyPoolNode", order = 0)]
    public class EnemyPoolNode : EncounterNode
    {
        public List<Creature> enemyPool;

        public float costMultiplier = 1f;
        public float extraCostMultiplier = 1f;
        public int spawnLimit = 9;

        public override List<Creature> GetEnemies() {
            List<Creature> list = new List<Creature>();

            float multiplier = costMultiplier;
            while (budget > 0 && list.Count < spawnLimit) {
                //dividing budget by cost multiplier is the same as multipling the cost
                Creature creature = GetCreature(budget/multiplier);
                if (creature == null) break;
                budget -= creature.spawnCost*multiplier;
                list.Add(creature);
                multiplier += extraCostMultiplier;
            }

            if (list.Count == 0) list.Add(enemyPool[0]);
            return list;
        }

        private Creature GetCreature(float currentBudget) {
            float affordable = 1f;
            Creature current = null;
            foreach (Creature enemy in enemyPool) {
                if (enemy.spawnCost <= currentBudget) {
                    // first affordable creature has a 100% chance to be used, next has 50%, then 33%, then 25%...
                    // this means a uniformly random creature that can be afforded out of the list will be
                    // since we dont know how many are affordable ahead of time, this is useful
                    if (Random.value <= 1f/affordable) {
                        current = enemy;
                    }
                    affordable++;
                }
            }
            return current;
        }
    }
}
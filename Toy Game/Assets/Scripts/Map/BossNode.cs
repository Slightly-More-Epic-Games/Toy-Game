using System.Collections;
using System.Collections.Generic;
using Items;
using UnityEngine;

namespace Map {
    [CreateAssetMenu(fileName = "BossNode", menuName = "Toy Game/Map/BossNode", order = 0)]
    public class BossNode : EncounterNode
    {
        public Creature boss;
        public List<Creature> minionPool;
        public int startingMinions;
        public int minionsPerLoop;

        public override List<Creature> GetEnemies() {
            // boss node doesnt use budget, instead it just always spawns the boss, then also spawns an increasing number of minions
            List<Creature> list = new List<Creature>();

            list.Add(boss);
            int minions = startingMinions + minionsPerLoop*Manager.instance.bossesKilled;

            for (int i = 0; i < minions; i++) {
                list.Add(minionPool[Random.Range(0, minionPool.Count)]);
            }

            return list;
        }
    }
}
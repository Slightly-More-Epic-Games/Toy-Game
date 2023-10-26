using System.Collections;
using System.Collections.Generic;
using Items;
using UnityEngine;

namespace Map {
    [CreateAssetMenu(fileName = "BossNode", menuName = "Toy Game/Map/BossNode", order = 0)]
    public class BossNode : EncounterNode
    {
        public Creature boss;
        public List<Creature> minions;

        public override List<Creature> GetEnemies() {
            List<Creature> list = new List<Creature>();

            list.Add(boss);
            list.AddRange(minions);

            return list;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map {
    public abstract class EncounterNode : Node
    {
        public int rewards;

        public abstract List<Creature> GetEnemies();
    }
}
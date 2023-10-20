using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Encounter {
    public class CreatureController : MonoBehaviour
    {
        public virtual void UpdateTurn(Creature owner) {}

        public virtual void OnTurnStart(Creature owner) {}

        public virtual void OnTurnEnd(Creature owner) {}

        public virtual void OnEncounterStart(Creature owner) {}

        public virtual void OnEncounterEnd(Creature owner) {}

        public virtual void OnEventsFinished(Creature owner) {}
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Encounter {
    public enum Action {
        ItemUsed,
        AnyItemUsed,
        DealDamage,
        GainHealth,
        LoseHealth,
        GainImagination,
        LoseImagination,
        TurnStart,
        TurnEnd,
        EncounterStart,
        EncounterEnd,
        LastStand,
        AnyDeath
    }
}
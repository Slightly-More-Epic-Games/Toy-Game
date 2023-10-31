using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Encounter {
    // this enum is for every type of event in the game
    // most of these get passed around to every item of every creature, but some (ItemUsed and LastStand) are only given to individual items and creatures
    // likewise some can be cancelled, some dont have meaningful sources or targets etc
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Encounter {
    public enum Action {
        ITEM_USED,
        ANY_ITEM_USED,
        DEAL_DAMAGE,
        GAIN_HEALTH,
        LOSE_HEALTH,
        GAIN_IMAGINATION,
        LOSE_IMAGINATION,
        TURN_START,
        TURN_END,
        ENCOUNTER_START,
        ENCOUNTER_END,
        LAST_STAND,
        ANY_DEATH
    }
}
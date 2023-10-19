using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Encounter {
    public enum Target {
        SOURCE,
        TARGET,
        SELF,
        ALL,
        RANDOM,
        ALL_NOT_SELF,
        RANDOM_NOT_SELF,
        ALL_OPPONENTS,
        RANDOM_OPPONENT,
        ALL_ALLIES,
        RANDOM_ALLY,
        LAST_ATTACKER
    }
}
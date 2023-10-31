using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Encounter {
    // used to describe a list of creatures relative to a context
    public enum Target {
        Source,
        Target,
        Self,
        All,
        Random,
        NotSelfAll,
        NotSelfRandom,
        OpponentsAll,
        OpponentsRandom,
        AlliesAll,
        AlliesRandom,
        LastAttacker
    }
}
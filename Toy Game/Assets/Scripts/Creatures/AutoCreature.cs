using System.Collections;
using System.Collections.Generic;
using Encounter;
using UnityEngine;

[CreateAssetMenu(fileName = "AutoCreature", menuName = "Toy Game/Creatures/Auto Creature", order = 0)]
public class AutoCreature : Creature {
    // the distinction between a playercreature, autocreature, and creature is sort of unnessecary, since the difference in controll is handed by CreatureControllers
    // ...however, it does mean in the future theres already a distinction, so the code should be easy to add to
}

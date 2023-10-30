using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map {
    [CreateAssetMenu(fileName = "StartingNode", menuName = "Toy Game/Map/StartingNode", order = 0)]
    public class StartingNode : Node
    {
        // the first node on the map cant be played but needs to exist for technical reasons
        // since the raw Node class cant be instantiated, an empty class like this should be used
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Encounter {
    public class PlayerController : MonoBehaviour
    {
        private void Update() {
            if (Input.GetKeyDown(KeyCode.Return)) {
                Manager.instance.AddEventToProcess(new Context(Action.TURN_END, Game.instance.player, Game.instance.player, 0));
                Manager.instance.ProcessEvents();
            }
        }
    }
}
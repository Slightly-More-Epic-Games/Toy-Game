using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Encounter {
    public class LoseManager : MonoBehaviour
    {
        public void Quit() {
            Game.instance.Quit();
        }
    }
}
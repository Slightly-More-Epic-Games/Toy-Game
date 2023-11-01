using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Encounter {
    public class LoseManager : MonoBehaviour
    {
        [SerializeField] private AudioClip deathMusic;

        private void Start() {
            Game.instance.Play(deathMusic);
        }

        public void Quit() {
            Game.instance.Quit();
        }
    }
}
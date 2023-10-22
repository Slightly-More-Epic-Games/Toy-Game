using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map {
    public class Node : ScriptableObject {
        protected float budget;

        public void Initialise(float budget) {
            this.budget = budget;
        }

        protected virtual void Initialised() {}
    }
}
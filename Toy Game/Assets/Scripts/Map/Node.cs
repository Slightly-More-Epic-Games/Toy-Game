using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map {
    public class Node : ScriptableObject {
        protected float budget;

        public List<int> connections;

        public Sprite icon;

        public void Initialise(float budget) {
            this.budget = budget;
            connections = new List<int>();
        }

        protected virtual void Initialised() {}
    }
}
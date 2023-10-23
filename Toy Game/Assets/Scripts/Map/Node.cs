using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map {
    public class Node : ScriptableObject {
        public Sprite icon;
        public Game.GameScene scene;
        
        protected float budget;

        [System.NonSerialized] public List<int> connections;
        [System.NonSerialized] public NodeVisual nodeVisual;

        public void Initialise(float budget) {
            this.budget = budget;
            connections = new List<int>();
        }

        protected virtual void Initialised() {}
    }
}
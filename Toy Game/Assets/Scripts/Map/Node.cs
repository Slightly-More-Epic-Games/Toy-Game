using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map {
    public class Node : ScriptableObject {
        public SpriteAnimation on;
        public SpriteAnimation off;
        
        public Game.GameScene scene;
        public float budgetToAppear;
        protected float budget;

        public AudioClip music;
        

        [System.NonSerialized] public List<int> connections;
        [System.NonSerialized] public NodeVisual nodeVisual;

        public void Initialise(float budget) {
            this.budget = budget;
            connections = new List<int>();
        }

        protected virtual void Initialised() {}
    }
}
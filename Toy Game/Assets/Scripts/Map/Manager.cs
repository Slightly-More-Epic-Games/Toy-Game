using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map {
    public class Manager : MonoBehaviour
    {
        public static Manager instance;

        public List<NodeRow> nodeRows;

        public void Start() {
            instance = this;
        }
    }
}
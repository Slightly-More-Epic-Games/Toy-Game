using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Map {
    public class NodeVisual : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Image image;

        private Node node;
        private NodeRow nodeRow;
        private int index;

        public void Initialise(Node node, NodeRow nodeRow, int index) {
            this.node = node;
            this.nodeRow = nodeRow;
            this.index = index;

            image.sprite = node.icon;
            button.onClick.AddListener(Play);
        }

        private void Play() {
            nodeRow.Play(index);
        }
    }
}
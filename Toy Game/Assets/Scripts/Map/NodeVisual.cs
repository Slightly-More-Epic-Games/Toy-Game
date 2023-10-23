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
        [SerializeField] private RectTransform child;
        [SerializeField] private float spreadRange;

        private Node node;
        private NodeRow nodeRow;
        private int index;

        public void Initialise(Node node, NodeRow nodeRow, int index) {
            this.node = node;
            this.nodeRow = nodeRow;
            this.index = index;

            image.sprite = node.icon;
            button.onClick.AddListener(Play);

            node.nodeVisual = this;

            Vector2 pos = Random.insideUnitCircle;
            child.anchoredPosition = new Vector3(pos.x*spreadRange, pos.y*spreadRange, 0);
        }

        private void Play() {
            nodeRow.Play(index);
        }
    }
}
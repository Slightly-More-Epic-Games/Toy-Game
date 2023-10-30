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

        private SpriteAnimation currentAnimation;
        private bool updateInteractable = true;

        // monobehaviours and scriptable objects are instantiated so dont really have constructors
        // therefore lots of them (like this one) instead have an initialise method
        public void Initialise(Node node, NodeRow nodeRow, int index) {
            this.node = node;
            this.nodeRow = nodeRow;
            this.index = index;

            button.onClick.AddListener(Play);

            node.nodeVisual = this;

            Vector2 pos = Random.insideUnitCircle;
            child.anchoredPosition = new Vector3(pos.x*spreadRange, pos.y*spreadRange, 0);

            currentAnimation = node.off;
        }

        private void Update() {
            image.sprite = currentAnimation.GetSprite();
        }

        private void Play() {
            Manager.instance.Play(nodeRow, node, index);
        }

        public void SetAnimation(SpriteAnimation animation) {
            // this is used to make the passed nodes empty
            // it is slightly ugly state logic though
            currentAnimation = animation;
            updateInteractable = false;
        }

        public void SetInteractable(bool interactable) {
            button.interactable = interactable;
            if (updateInteractable) {
                currentAnimation = interactable ? node.on : node.off;
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Map {
    public class Manager : MonoBehaviour
    {
        public static Manager instance;
        [System.NonSerialized] public Node currentNode;
        private int currentLevel = 0;
        [System.NonSerialized] public int bossesKilled;

        private List<NodeRow> nodeRows = new List<NodeRow>();
        private int totalRows = 0;

        [SerializeField] private List<Node> nodeTemplates;
        [SerializeField] private List<Node> bossNodeTemplates;
        [SerializeField] private List<Node> specialNodes;
        [SerializeField] private Node startingNode;


        [SerializeField] private Transform nodeRowParents;
        [SerializeField] private Transform nodeRowPrefab;
        [SerializeField] private NodeVisual nodePrefab;

        [SerializeField] private GameObject dontDestroy;

        [SerializeField] private RectTransform rowLayoutGroup;

        [SerializeField] private LineRenderer linePrefab;

        [SerializeField] private Image playerIndicator;
        [SerializeField] private float walkSpeed;
        private bool walking;

        [SerializeField] private SpriteAnimation empty;

        [SerializeField] private AudioClip mapMusic;


        private void Awake() {
            // map manager stays loaded, so if an instance already exists, destroy the new one
            if (instance != null) {
                DestroyManager();
                return;
            }

            DontDestroyOnLoad(this);
            DontDestroyOnLoad(dontDestroy);
            instance = this;     
        }

        private void Start() {
            CreateMap();
        }

        private void Update() {
            if (!gameObject.activeSelf) return;
            // update player position on the map
            playerIndicator.sprite = Game.instance.player.spriteAnimation.GetSprite();
            Vector3 target = currentNode.nodeVisual.transform.GetChild(0).position + new Vector3(0,14,0);
            if (walking) {
                playerIndicator.transform.position = Vector3.MoveTowards(playerIndicator.transform.position, target, walkSpeed*Time.deltaTime);
                if (Vector3.Distance(playerIndicator.transform.position, target) < 1) {
                    currentNode.nodeVisual.SetAnimation(empty);
                    Game.instance.LoadGameScene(currentNode.scene);
                }
            } else {
                playerIndicator.transform.position = target;
            }
        }

        private void CreateMap() {
            if (nodeRows.Count != 0) {
                // if theres already a map but create map is being called, then that must be because a boss was just killed
                bossesKilled++;
                foreach (NodeRow nodeRow in nodeRows) {
                    nodeRow.Destroy();
                }
                nodeRows.Clear();
            }

            currentLevel = 0;
            // first node is a starting node
            CreateNextNodeRow(1, new List<Node>() {startingNode}, null);
            nodeRows[0].current = 0;
            currentNode = nodeRows[0].nodes[0];
            for (int i = 0; i < 6; i++) {
                // every other row has a shop
                CreateNextNodeRow(Random.Range(2,5), nodeTemplates, i%2 == 1 ? specialNodes : null);
            }
            // last node is a boss
            CreateNextNodeRow(1, bossNodeTemplates, null);

            // update what nodes are interactable
            UpdateMap();
        }

        public void DestroyManager() {
            Destroy(gameObject);
            Destroy(dontDestroy);
        }

        public void SetManagerActive(bool active) {
            gameObject.SetActive(active);
            dontDestroy.SetActive(active);
            if (!active) return;
            // when returning to the map, update what nodes are interactable
            UpdateMap();
        }

        private void CreateNextNodeRow(int count, List<Node> nodeTemplates, List<Node> specialNodes) {
            // make a new row
            NodeRow nodeRow = new NodeRow(Game.instance.player.spawnCost*totalRows, nodeTemplates, specialNodes, count);
            totalRows++;
            // create all the buttons
            nodeRow.CreateButtons(nodeRowPrefab, nodeRowParents, nodePrefab);
            // update the layout group
            LayoutRebuilder.ForceRebuildLayoutImmediate(rowLayoutGroup);
            if (nodeRows.Count > 0) {
                // connect to last
                NodeRow prev = nodeRows[nodeRows.Count-1];
                prev.Connect(nodeRow);
                prev.CreateConnections(nodeRow, linePrefab);
            }
            nodeRows.Add(nodeRow);
        }

        private void UpdateMap() {
            // this code is not very readable
            walking = false;
            List<int> nextConnections = null;
            foreach (NodeRow nodeRow in nodeRows) {
                // make all nodes not interactable
                foreach (Node node in nodeRow.nodes) {
                    node.nodeVisual.SetInteractable(false);
                }
                if (nextConnections != null) {
                    // make each node thats got a connection with the previous node interactable
                    foreach (int connection in nextConnections) {
                        nodeRow.nodes[connection].nodeVisual.SetInteractable(true);
                    }
                }
                // once the current node has been found set the connections for next row
                if (nodeRow.current != -1) {
                    nextConnections = nodeRow.nodes[nodeRow.current].connections;
                } else {
                    nextConnections = null;
                }
            }

            //reached end of map
            if (nextConnections != null && nextConnections.Count == 0) {
                Game.instance.player.health = Game.instance.player.maxHealth;
                CreateMap();
            }

            Game.instance.Play(mapMusic);
        }

        public void Play(NodeRow nodeRow, Node node, int index) {
            nodeRows[currentLevel].current = -1;
            nodeRow.current = index;
            currentLevel++;
            currentNode = node;
            walking = true;
            // this happens before the next scene is actually loaded, which makes it sound more seamless
            Game.instance.Play(node.music);
        }
    }
}
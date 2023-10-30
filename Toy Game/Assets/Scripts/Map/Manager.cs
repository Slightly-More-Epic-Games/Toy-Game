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
                bossesKilled++;
                foreach (NodeRow nodeRow in nodeRows) {
                    nodeRow.Destroy();
                }
                nodeRows.Clear();
            }
            currentLevel = 0;
            CreateNextNodeRow(1, new List<Node>() {startingNode}, null);
            nodeRows[0].current = 0;
            currentNode = nodeRows[0].nodes[0];
            for (int i = 0; i < 6; i++) {
                CreateNextNodeRow(Random.Range(2,5), nodeTemplates, i%2 == 1 ? specialNodes : null);
            }
            CreateNextNodeRow(1, bossNodeTemplates, null);
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
            UpdateMap();
        }

        private void CreateNextNodeRow(int count, List<Node> nodeTemplates, List<Node> specialNodes) {
            NodeRow nodeRow = new NodeRow(Game.instance.player.spawnCost*totalRows, nodeTemplates, specialNodes, count);
            totalRows++;
            nodeRow.CreateButtons(nodeRowPrefab, nodeRowParents, nodePrefab);
            LayoutRebuilder.ForceRebuildLayoutImmediate(rowLayoutGroup);
            if (nodeRows.Count > 0) {
                NodeRow prev = nodeRows[nodeRows.Count-1];
                prev.Connect(nodeRow);
                prev.CreateConnections(nodeRow, linePrefab);
            }
            nodeRows.Add(nodeRow);
        }

        private void UpdateMap() {
            walking = false;
            List<int> nextConnections = null;
            foreach (NodeRow nodeRow in nodeRows) {
                foreach (Node node in nodeRow.nodes) {
                    node.nodeVisual.SetInteractable(false);
                }
                if (nextConnections != null) {
                    foreach (int connection in nextConnections) {
                        nodeRow.nodes[connection].nodeVisual.SetInteractable(true);
                    }
                }
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
            Game.instance.Play(node.music);
        }
    }
}
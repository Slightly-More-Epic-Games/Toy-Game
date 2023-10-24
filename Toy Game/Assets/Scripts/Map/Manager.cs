using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Map {
    public class Manager : MonoBehaviour
    {
        public static Manager instance;
        public Node currentNode;
        public int currentLevel = 0;

        private List<NodeRow> nodeRows = new List<NodeRow>();
        private int rowCount = 1;

        [SerializeField] private List<Node> nodeTemplates;
        [SerializeField] private List<Node> bossNodeTemplates;
        [SerializeField] private Node startingNode;


        [SerializeField] private Transform nodeRowParents;
        [SerializeField] private Transform nodeRowPrefab;
        [SerializeField] private NodeVisual nodePrefab;

        [SerializeField] private GameObject dontDestroy;

        [SerializeField] private RectTransform rowLayoutGroup;

        [SerializeField] private LineRenderer linePrefab;

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
            CreateNextNodeRow(1, new List<Node>() {startingNode});
            nodeRows[0].current = 0;
            currentNode = nodeRows[0].nodes[0];
            for (int i = 0; i < 6; i++) {
                CreateNextNodeRow(Random.Range(2,5), nodeTemplates);
            }
            CreateNextNodeRow(1, bossNodeTemplates);
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

        private void CreateNextNodeRow(int count, List<Node> nodeTemplates) {
            NodeRow nodeRow = new NodeRow(Game.instance.player.spawnCost*rowCount, nodeTemplates, count);
            rowCount++;
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
        }

        public void Play(NodeRow nodeRow, Node node, int index) {
            nodeRows[currentLevel].current = -1;
            nodeRow.current = index;
            currentLevel++;
            currentNode = node;
            Game.instance.LoadGameScene(node.scene);
        }
    }
}
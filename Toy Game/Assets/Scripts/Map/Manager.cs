using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Map {
    public class Manager : MonoBehaviour
    {
        public static Manager instance;

        private List<NodeRow> nodeRows = new List<NodeRow>();
        private int rowCount = 1;

        [SerializeField] private List<Node> nodeTemplates;

        [SerializeField] private Transform nodeRowParents;
        [SerializeField] private Transform nodeRowPrefab;
        [SerializeField] private NodeVisual nodePrefab;

        [SerializeField] private GameObject dontDestroy;

        [SerializeField] private RectTransform rowLayoutGroup;

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
            for (int i = 0; i < 5; i++) {
                CreateNextNodeRow(Random.Range(2,5));
            }
        }

        public void DestroyManager() {
            Destroy(gameObject);
            Destroy(dontDestroy);
        }

        public void SetManagerActive(bool active) {
            gameObject.SetActive(active);
            dontDestroy.SetActive(active);
        }

        private void CreateNextNodeRow(int count) {
            NodeRow nodeRow = new NodeRow(Game.instance.player.spawnCost*rowCount, nodeTemplates, count);
            rowCount++;
            CreateButtons(nodeRow);
            if (nodeRows.Count > 0) {
                NodeRow prev = nodeRows[nodeRows.Count-1];
                prev.Connect(nodeRow);
                CreateConnections(prev, nodeRow);
            }
            nodeRows.Add(nodeRow);
        }

        private void CreateButtons(NodeRow nodeRow) {
            Transform row = Instantiate(nodeRowPrefab, nodeRowParents);
            for (int i = 0; i < nodeRow.nodes.Count; i++) {
                Node node = nodeRow.nodes[i];
                NodeVisual nodeVisual = Instantiate(nodePrefab, row);
                nodeVisual.Initialise(node, nodeRow, i);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(rowLayoutGroup);
        }

        private void CreateConnections(NodeRow from, NodeRow to) {
            //create lines
        }
    }
}
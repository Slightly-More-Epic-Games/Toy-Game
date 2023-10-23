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
            CreateNextNodeRow(1);
            for (int i = 0; i < 6; i++) {
                CreateNextNodeRow(Random.Range(2,5));
            }
            CreateNextNodeRow(1);
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
            nodeRow.CreateButtons(nodeRowPrefab, nodeRowParents, nodePrefab);
            LayoutRebuilder.ForceRebuildLayoutImmediate(rowLayoutGroup);
            if (nodeRows.Count > 0) {
                NodeRow prev = nodeRows[nodeRows.Count-1];
                prev.Connect(nodeRow);
                prev.CreateConnections(nodeRow, linePrefab);
            }
            nodeRows.Add(nodeRow);
        }
    }
}
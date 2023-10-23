using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Map {
    public class NodeRow {
        public float budget;

        public List<Node> nodes;

        public Transform row;

        public NodeRow(float budget, List<Node> nodeTemplates, int count) {
            this.budget = budget;
            nodes = new List<Node>(count);
            for (int i = 0; i < count; i++) {
                Node node = Object.Instantiate(nodeTemplates[Random.Range(0, nodeTemplates.Count)]);
                node.Initialise(budget);
                nodes.Add(node);
            }
        }

        public void Connect(NodeRow other) {
            for (int c = 0; c < nodes.Count; c++) {
                Node current = nodes[c];
                float currentPos = CenteredPosition(c);

                for (int o = 0; o < other.nodes.Count; o++) {
                    float newPos = other.CenteredPosition(o);
                    float newDist = Mathf.Abs(currentPos-newPos);
                    if (newDist < 0.75f || (newDist < 1.25f && Random.value <= 0.5f)) {
                        current.connections.Add(o);
                    } else if (other.nodes.Count > nodes.Count && ((c == 0 && newPos < currentPos) || (c == nodes.Count-1 && newPos > currentPos))) {
                        current.connections.Add(o);
                    }
                }

                if (current.connections.Count == 0) {
                    if (currentPos > 0) {
                        current.connections.Add(other.nodes.Count-1);
                    } else {
                        current.connections.Add(0);
                    }
                }
            }
        }

        public float CenteredPosition(int index) {
            return index-((nodes.Count-1)/2f);
        }

        public void Play(int index) {
            Debug.Log(nodes[index]);
        }

        public void CreateButtons(Transform nodeRowPrefab, Transform nodeRowParents, NodeVisual nodePrefab) {
            row = Object.Instantiate(nodeRowPrefab, nodeRowParents);
            for (int i = 0; i < nodes.Count; i++) {
                Node node = nodes[i];
                NodeVisual nodeVisual = Object.Instantiate(nodePrefab, row);
                nodeVisual.Initialise(node, this, i);
            }
        }

        public void CreateConnections(NodeRow other, LineRenderer linePrefab) {
            foreach (Node node in nodes) {
                foreach (int connection in node.connections) {
                    LineRenderer line = Object.Instantiate(linePrefab, node.nodeVisual.transform);
                    line.transform.localPosition = Vector3.zero;
                    line.SetPosition(1, other.nodes[connection].nodeVisual.transform.position - node.nodeVisual.transform.position + new Vector3(16,0,0));
                }
            }
        }
    }
}
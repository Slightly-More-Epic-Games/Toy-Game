using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Map {
    public class NodeRow {
        public float budget;

        public List<Node> nodes;

        public Transform row;

        public int current;

        public NodeRow(float budget, List<Node> nodeTemplates, List<Node> specialNodes, int count) {
            // this code is a little complicated
            // the overall goal is to create one vertical row of levels on the map

            // the budget increases for each row on the map
            // it is used to determine which nodes to spawn and what appears in each node
            this.budget = budget;

            nodes = new List<Node>(count);

            // special nodes will always appear exactly once at a random point - if there are any
            // this is currently only used by shops - but the system is general
            int specialIndex = -1;
            if (specialNodes != null) {
                specialIndex = Random.Range(0, count);
            }
            
            // creates count nodes to fill out the row
            for (int i = 0; i < count; i++) {
                Node node;
                if (i == specialIndex) {
                    // if the current index is the predetermined special index, spawn a random special node
                    node = Object.Instantiate(specialNodes[Random.Range(0, specialNodes.Count)]);
                } else {
                    // otherwise, spawn a node provided it is within the budget for this row
                    // (some nodes require higher budgets to spawn)
                    node = Object.Instantiate(GetNodeWithinBudget(nodeTemplates, budget));
                }

                node.Initialise(budget);
                nodes.Add(node);
            }
            current = -1;
        }

        public Node GetNodeWithinBudget(List<Node> nodeTemplates, float budget) {
            // pick a random index in the template list
            // if its to expensive, keep going back up the list until its affordable
            int index = Random.Range(0, nodeTemplates.Count);
            Node node = nodeTemplates[index];
            while (node.budgetToAppear > budget && index > 0) {
                index--;
                node = nodeTemplates[index];
            }
            return node;
        }

        public void Connect(NodeRow other) {
            // this creates all the connections for the next row over
            for (int c = 0; c < nodes.Count; c++) {
                Node current = nodes[c];
                float currentPos = CenteredPosition(c);

                for (int o = 0; o < other.nodes.Count; o++) {
                    float newPos = other.CenteredPosition(o);
                    float newDist = Mathf.Abs(currentPos-newPos);
                    if (newDist < 0.75f || (newDist < 1.25f && Random.value <= 0.5f)) {
                        // connect nodes if they are directly aligned, form a triangle, or sometimes if they are diagonal
                        current.connections.Add(o);
                    } else if (other.nodes.Count > nodes.Count && ((c == 0 && newPos < currentPos) || (c == nodes.Count-1 && newPos > currentPos))) {
                        // if the current row is small and the next one is larger, then the topmost node will have to connect to all nodes higher than it, and likewise for the bottom
                        // if this didnt happen, some nodes could end up without ways of entering them
                        current.connections.Add(o);
                    }
                }

                // if a node has no out connections, its because the current row is bigger than the next
                // all of these can therefore be attached to the topmost of the smaller row
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

        public void CreateButtons(Transform nodeRowPrefab, Transform nodeRowParents, NodeVisual nodePrefab) {
            // create all the buttons for the node row
            row = Object.Instantiate(nodeRowPrefab, nodeRowParents);
            for (int i = 0; i < nodes.Count; i++) {
                Node node = nodes[i];
                NodeVisual nodeVisual = Object.Instantiate(nodePrefab, row);
                if (node as BossNode != null) {
                    // if its a boss node, make the node bigger
                    // "size" should probably be a property of each node
                    nodeVisual.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(48, 48);
                }
                nodeVisual.Initialise(node, this, i);
            }
        }

        public void CreateConnections(NodeRow other, LineRenderer linePrefab) {
            // create all the lines for the connections previously calculated
            foreach (Node node in nodes) {
                Vector3 nodePos = node.nodeVisual.transform.GetChild(0).position;
                foreach (int connection in node.connections) {
                    LineRenderer line = Object.Instantiate(linePrefab, node.nodeVisual.transform);
                    line.transform.localPosition = node.nodeVisual.transform.GetChild(0).localPosition;
                    line.SetPosition(1, other.nodes[connection].nodeVisual.transform.GetChild(0).position - nodePos + new Vector3(16,0,0));
                }
            }
        }

        public void Destroy() {
            // gets rid of all old stuff on the map - needed when a new map is being generated
            Object.Destroy(row.gameObject);
        }
    }
}
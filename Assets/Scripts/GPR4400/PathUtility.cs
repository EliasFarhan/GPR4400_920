using System.Collections;
using System.Collections.Generic;
using Priority_Queue;
using UnityEngine;

namespace path
{
    public struct Neighbor
    {
        public int neighbor;
        public float length;

        public Neighbor(int neighbor, float length)
        {
            this.neighbor = neighbor;
            this.length = length;
        }
    }
    /// <summary>Simple node struct used for pathfinding</summary>
    public struct Node
    {
        public Vector2 position;
        public List<Neighbor> neighbors;

        public Node(Vector2 position)
        {
            this.position = position;
            neighbors = new List<Neighbor>();
        }
        
    }
    /**
     * <summary>Bidirectional graph</summary> 
     */
    public class Graph
    {
        private List<Node> nodes_ = new List<Node>();

        public int AddNode(Vector2 position)
        {
            int index = nodes_.Count;
            nodes_.Add(new Node(position));
            return index;
        }
        /**
         * <summary>Add neighbor node2 to node1 neighbors list.
         * Neighbors are represented as node indexes</summary>
         */
        public void AddNeighborEdge(int node1, int node2, bool bidirectional=false)
        {
            var distance = (nodes_[node2].position - nodes_[node1].position).magnitude;
            nodes_[node1].neighbors.Add(new Neighbor(node2, distance));
            if (bidirectional)
            {
                nodes_[node2].neighbors.Add(new Neighbor(node1, distance));
            }
        }
 
        public void Clear()
        {
            nodes_.Clear();
        }
        /**
         * <summary>Calculate the shortest path from the startNode to the destinationNode.
         * Using A* pathfinding algorithm.</summary>
         */
        public List<int> CalculatePath(int startNode, int destinationNode)
        {
            var path = new List<int>();
            Dictionary<int, int> cameFrom = new Dictionary<int, int> {[startNode] = -1};
            Dictionary<int, float> costSoFar = new Dictionary<int, float> {[startNode] = 0};
            SimplePriorityQueue<int> frontier = new SimplePriorityQueue<int>();
            frontier.Enqueue(startNode, 0.0f);
            var destinationPosition = nodes_[destinationNode].position;
            while (frontier.Count > 0)
            {
                var currentNode = frontier.Dequeue();
                if (currentNode == destinationNode)
                {
                    break;
                }
                foreach (var neighbor in nodes_[currentNode].neighbors)
                {
                    var newCost = costSoFar[currentNode] + neighbor.length;
                    if (costSoFar.ContainsKey(neighbor.neighbor) && 
                        !(newCost < costSoFar[neighbor.neighbor])) continue;
                    
                    costSoFar[neighbor.neighbor] = newCost;
                    var priority = newCost + (destinationPosition - nodes_[neighbor.neighbor].position).magnitude;
                    if (frontier.Contains(neighbor.neighbor))
                    {
                        frontier.UpdatePriority(neighbor.neighbor, priority);
                    }
                    else
                    {
                        frontier.Enqueue(neighbor.neighbor, priority);
                    }

                    cameFrom[neighbor.neighbor] = currentNode;
                }
            }

            if (!cameFrom.ContainsKey(destinationNode))
            {
                return path;
            }
            path.Add(destinationNode);
            var tmpNode = destinationNode;
            while (tmpNode != startNode)
            {
                tmpNode = cameFrom[tmpNode];
                path.Add(tmpNode);
            }
            path.Reverse();
            return path;
        }
    }
}
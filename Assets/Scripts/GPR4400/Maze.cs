
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour
{
    [SerializeField] private Tile tilePrefab;

    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;

    private const float tileSize = 1.0f;
    private List<Tile> tiles_ = new List<Tile>();
    struct Node
    {
        public int index;
        public Vector2Int pos;
        public Vector2 worldPos;
        public List<int> neighbors;

        public Node(Vector2Int pos, Vector2 worldPos, int index)
        {
            this.pos = pos;
            this.worldPos = worldPos;
            this.index = index;
            neighbors = new List<int>(4);
        }
    }
    class Graph
    {
        private List<Node> nodes_ = new List<Node>();
        public List<Node> Nodes => nodes_;
        public int AddNode(Vector2Int pos, Vector2 worldPos)
        {
            int index = nodes_.Count;
            nodes_.Add(new Node(pos, worldPos, index));
            return nodes_.Count-1;
        }

        public void AddNeighbor(int start, int end)
        {
            var startNode = nodes_[start];
            var endNode = nodes_[end];
            
            startNode.neighbors.Add(end);
            endNode.neighbors.Add(start);

            nodes_[start] = startNode;
            nodes_[end] = endNode;
        }

        public bool AreNeighbor(int start, int end)
        {
            return nodes_[start].neighbors.Contains(end);
        }
    }

    private Graph graph_;
    void GenerateMaze()
    {
        graph_ = new Graph();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var pos = new Vector2Int(x, y);
                Vector2 worldPos = new Vector2(2.0f*x-width, 2.0f*y-height)*tileSize+Vector2.one*tileSize;
                graph_.AddNode(pos, worldPos);
                AddTile(worldPos, false);
            }
        }

        int[] comeFrom = new int[graph_.Nodes.Count];
        for (int i = 0; i < comeFrom.Length; i++)
        {
            comeFrom[i] = -1;
        }

        Vector2Int[] delta = {
            Vector2Int.right, Vector2Int.up
        };

        List<int> nextNodeIndexes = new List<int>();
        nextNodeIndexes.Add(0);
        while (nextNodeIndexes.Count > 0)
        {
            var randomIndex = Random.Range(0, nextNodeIndexes.Count);
            var nodeIndex = nextNodeIndexes[randomIndex];
            nextNodeIndexes.RemoveAt(randomIndex);
            var node = graph_.Nodes[nodeIndex];

            foreach (var d in delta)
            {
                var neighborPos = node.pos + d;
                if (neighborPos.x >= width || neighborPos.y >= height)
                {
                    continue;
                }
                var neighborIndex = neighborPos.x * height + neighborPos.y;
                if(comeFrom[neighborIndex] != -1)
                    continue;
                nextNodeIndexes.Add(neighborIndex);
                comeFrom[neighborIndex] = nodeIndex;
                graph_.AddNeighbor(nodeIndex, neighborIndex);
            }
        }
    }

    void AddTile(Vector3 worldPos, bool wall)
    {
        var tile = Instantiate(tilePrefab, worldPos, Quaternion.identity, transform);
        tile.SpriteRenderer.color = wall ? Color.black : Color.white;
        if (wall)
        {
            tile.gameObject.AddComponent<BoxCollider2D>();
        }
        tiles_.Add(tile);
    }
    void InstantiateGraph()
    {
        
        for (int x = 0; x <= width; x++)
        {
            var worldPos = new Vector2(2.0f * x - width, -height) * tileSize;
            AddTile(worldPos, true);
            if (x != width)
            {
                worldPos = new Vector2(2.0f * x + 1 - width, -height)* tileSize;
                AddTile(worldPos, x!=0);
            }

            worldPos = new Vector2(2.0f * x  - width, height)* tileSize;
            AddTile(worldPos, true);
            if (x != width)
            {
                worldPos = new Vector2(2.0f * x * 1 + tileSize - width, height)* tileSize;
                AddTile( worldPos, x!=width-1);
            }
        }

        for (int y = 1; y < height; y++)
        {
            var worldPos = new Vector2(-width, 2.0f*y-height)* tileSize;
            AddTile( worldPos, true);

            worldPos = new Vector2(-width, 2.0f*y-1-height)* tileSize;
            AddTile( worldPos, true);
            if (y == height - 1)
            {
                worldPos = new Vector2(-width, 2.0f*y+1-height)* tileSize;
                AddTile( worldPos, true);
            }
            
            worldPos = new Vector2(width, 2.0f*y-height)* tileSize;
            AddTile(worldPos, true);

            worldPos = new Vector2(width, 2.0f*y-1-height)* tileSize;
            AddTile( worldPos, true);
            if (y == height - 1)
            {
                worldPos = new Vector2(width, 2.0f*y+1-height)* tileSize;
                AddTile( worldPos, true);
            }
            
        }

        for (int x = 1; x < width; x++)
        {
            for (int y = 1; y < height; y++)
            {
                var worldPos = new Vector2(2.0f*x-width, 2.0f*y-height)* tileSize;
                AddTile(worldPos, true);
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var nodeIndex = x * height + y;
                if (x < width - 1)
                {
                    var rightIndex = (x + 1) * height + y;
                    var worldPos = graph_.Nodes[nodeIndex].worldPos + Vector2.right * tileSize;
                    AddTile( worldPos, !graph_.AreNeighbor(nodeIndex, rightIndex));
                }

                if (y < height - 1)
                {
                    var topIndex = x * height + y + 1;
                    var worldPos = graph_.Nodes[nodeIndex].worldPos + Vector2.up * tileSize;
                    AddTile( worldPos, !graph_.AreNeighbor(nodeIndex, topIndex));
                }
            }
        }
    }

    private void Start()
    {
        GenerateMaze();
        InstantiateGraph();
    }
}

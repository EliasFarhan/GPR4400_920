using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GPR4400
{
    public class CellularAutomataWithHotPath : CellularAutomata
    {
        private Vector2Int startingPoint;
        private Vector2Int endPoint;
        private List<Vector2Int> path;
        private int[,] distanceFromPath;
        [SerializeField] private int hotPathThreshold = 10;
        protected override void Init()
        {
            base.Init();
            GeneratePath();
            GenerateDistanceFromPath();
        }



        protected override void ConnectClosestRegions()
        {
            //Add starting and end region
            startingPoint = new Vector2Int(0, Random.Range(0, height));
            cells[startingPoint.x, startingPoint.y].isAlive = true;
            cellViews[startingPoint.x, startingPoint.y].IsAlive = true;
            Region startingRegion = new Region();
            startingRegion.AddTile(startingPoint);
            regions_.Add(startingRegion);
            
            endPoint = new Vector2Int(width-1, Random.Range(0, height));
            cells[endPoint.x, endPoint.y].isAlive = true;
            cellViews[endPoint.x, endPoint.y].IsAlive = true;
            Region endRegion = new Region();
            endRegion.AddTile(endPoint);
            regions_.Add(endRegion);
            
            base.ConnectClosestRegions();
        }

        void GeneratePath()
        {
            path = new List<Vector2Int>();
            Queue<Vector2Int> nextPosition = new Queue<Vector2Int>();
            Dictionary<Vector2Int, Vector2Int> comeFrom = new Dictionary<Vector2Int, Vector2Int>();
            nextPosition.Enqueue(startingPoint);
            var deltas = new Vector2Int[]
            {
                Vector2Int.right,
                Vector2Int.up, 
                Vector2Int.down,
                Vector2Int.left
            };
            Vector2Int currentPos = Vector2Int.zero;
            
            while (nextPosition.Count > 0)
            {
                currentPos = nextPosition.Dequeue();
                if (currentPos == endPoint)
                {
                    break;
                }

                foreach (var delta in deltas)
                {
                    var neighborPos = currentPos + delta;
                    if(neighborPos.x < 0 || neighborPos.y < 0 || neighborPos.x >= width || neighborPos.y >= height)
                        continue;
                    if (neighborPos == startingPoint || comeFrom.ContainsKey(neighborPos))
                    {
                        continue;
                    }
                    if(!cells[neighborPos.x, neighborPos.y].isAlive)
                        continue;
                    comeFrom[neighborPos] = currentPos;
                    nextPosition.Enqueue(neighborPos);
                }
                
            }

            if (currentPos != endPoint)
            {
                throw new Exception("No path found!");
                return;
            }

            while (currentPos != startingPoint)
            {
                path.Add(currentPos);
                currentPos = comeFrom[currentPos];
            }
            path.Add(startingPoint);
            path.Reverse();
            foreach (var point in path)
            {
                cellViews[point.x, point.y].UpdateColor(Color.red);
            }

        }
        private void GenerateDistanceFromPath()
        {
            distanceFromPath = new int[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    distanceFromPath[i, j] = -1;
                }
            }

            foreach (var point in path)
            {
                distanceFromPath[point.x, point.y] = 0;
            }
            
            foreach (var pathPoint in path)
            {
                Queue<Vector2Int> nextPosition = new Queue<Vector2Int>();
                Dictionary<Vector2Int, int> costSoFar = new Dictionary<Vector2Int, int>();
                costSoFar[pathPoint] = 0;
                nextPosition.Enqueue(pathPoint);
                var deltas = new[]
                {
                    Vector2Int.right,
                    Vector2Int.up, 
                    Vector2Int.down,
                    Vector2Int.left
                };

                while (nextPosition.Count > 0)
                {
                    var currentPos = nextPosition.Dequeue();

                    foreach (var delta in deltas)
                    {
                        var neighborPos = currentPos + delta;
                        if(neighborPos.x < 0 || neighborPos.y < 0 || neighborPos.x >= width || neighborPos.y >= height)
                            continue;
                        if (costSoFar.ContainsKey(neighborPos))
                        {
                            continue;
                        }
                        if(!cells[neighborPos.x, neighborPos.y].isAlive)
                            continue;
                        var cost = costSoFar[currentPos]+1;
                        costSoFar[neighborPos] = cost;
                        if (distanceFromPath[neighborPos.x, neighborPos.y] == -1 ||
                             cost < distanceFromPath[neighborPos.x, neighborPos.y])
                        {
                            distanceFromPath[neighborPos.x, neighborPos.y] = cost;
                            cellViews[neighborPos.x, neighborPos.y].UpdateColor(
                                cost < hotPathThreshold ? Color.yellow : Color.gray);
                        }
                        nextPosition.Enqueue(neighborPos);
                    }
                }
            }
        }
    }
}
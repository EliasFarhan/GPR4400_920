using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[ExecuteInEditMode]
public class PoissonDiskSample : MonoBehaviour
{
    private List<Vector2> points = new List<Vector2>();

    [SerializeField] private float radius = 1.0f;

    [SerializeField] private int numSamplesBeforeRejection = 10;

    // Start is called before the first frame update
    void Start()
    {
        GeneratePoissonDiskSampling();
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.isEditor && !Application.isPlaying)
        {
            GeneratePoissonDiskSampling();
        }
    }

    void GeneratePoissonDiskSampling()
    {
        System.DateTime start = System.DateTime.Now;

        var mainCamera = Camera.main;
        var cameraSize = 2.0f * mainCamera.orthographicSize * new Vector2(mainCamera.aspect, 1.0f);
        var cameraRect = new Rect(){min=-cameraSize/2.0f, max = cameraSize/2.0f};
        var cellSize = radius / Mathf.Sqrt(2);
        
        points.Clear();
        
        int[,] grid = new int[Mathf.CeilToInt(cameraRect.width / cellSize),
            Mathf.CeilToInt(cameraRect.height / cellSize)];

        bool IsValid(Vector2 candidatePoint)
        {
            if (candidatePoint.x < cameraRect.xMin || candidatePoint.y < cameraRect.yMin ||
                candidatePoint.x > cameraRect.xMax || candidatePoint.y > cameraRect.yMax)
            {
                return false;
            }

            Vector2Int cellPos = new Vector2Int(
                (int) ((candidatePoint.x + cameraRect.width / 2.0f) / cellSize),
                (int) ((candidatePoint.y + cameraRect.height / 2.0f) / cellSize));

            Vector2Int searchStart = new Vector2Int(Mathf.Max(0, cellPos.x - 2),
                Mathf.Max(0, cellPos.y - 2));
            Vector2Int searchEnd = new Vector2Int(Mathf.Min(cellPos.x + 2, grid.GetLength(0) - 1),
                Mathf.Min(cellPos.y + 2, grid.GetLength(1) - 1));
            for (int x = searchStart.x; x <= searchEnd.x; x++)
            {
                for (int y = searchStart.y; y <= searchEnd.y; y++)
                {
                    int pointIndex = grid[x, y] - 1;
                    if (pointIndex != -1)
                    {
                        float sqrDistance = (candidatePoint - points[pointIndex]).sqrMagnitude;
                        if (sqrDistance < radius*radius)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        
        List<Vector2> spawnPoints = new List<Vector2>();
        spawnPoints.Add(Vector2.zero);
        while (spawnPoints.Count > 0)
        {
            bool candidateAccepted = false;
            int spawnIndex = Random.Range(0, spawnPoints.Count);
            Vector2 spawnCenter = spawnPoints[spawnIndex];

            for (int i = 0; i < numSamplesBeforeRejection; i++)
            {
                float angle = Random.value * Mathf.PI * 2.0f;
                Vector2 dir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
                Vector2 candidatePoint = spawnCenter + dir * Random.Range(radius, 2.0f * radius);
                if (!IsValid(candidatePoint)) continue;
                
                points.Add(candidatePoint);
                spawnPoints.Add(candidatePoint);
                grid[(int) ((candidatePoint.x+cameraRect.width/2.0f) / cellSize), 
                    (int) ((candidatePoint.y+cameraRect.height/2.0f) / cellSize)] = points.Count;
                candidateAccepted = true;
                break;
            }

            if (!candidateAccepted)
            {
                spawnPoints.RemoveAt(spawnIndex);
            }
        }
        System.DateTime end = System.DateTime.Now;
        System.TimeSpan ts = (end - start);
        Debug.Log("Generate Points Elapsed Time is "+ts.TotalMilliseconds+"ms");
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (var point in points)
        {
            Gizmos.DrawSphere(point, radius/2.0f);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DrunkardHulkGenerator : MonoBehaviour
{
    [SerializeField] private Tile tilePrefab;

    [SerializeField] private int width = 0;
    [SerializeField] private int height = 0;

    [SerializeField] private int step = 10;

    private Tile[,] tileViews;

    private bool[,] tileStatus;

    private List<Vector2Int> brokenTiles;
    // Start is called before the first frame update
    void Start()
    {
        tileViews = new Tile[width, height];
        tileStatus = new bool[width, height];
        brokenTiles = new List<Vector2Int>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 worldPos = new Vector2(x-width/2.0f, y-height/2.0f)+Vector2.one*0.5f;
                var tile = Instantiate(tilePrefab, worldPos, Quaternion.identity, transform);
                tile.SpriteRenderer.color = Color.black;
                tileViews[x, y] = tile;
                tileStatus[x, y] = false;
            }
        }
    }

    void GenerateHulkDrunkard(Vector2Int startingPos)
    {
        var hulkPos = startingPos;
        var newmanNeighbors = new Vector2Int[4]
        {
            Vector2Int.right, 
            Vector2Int.left, 
            Vector2Int.down, 
            Vector2Int.up
        };
        for (int i = 0; i < step; i++)
        {
            hulkPos += newmanNeighbors[Random.Range(0, newmanNeighbors.Length)];
            if (hulkPos.x < 0 || hulkPos.y < 0 || hulkPos.x >= width || hulkPos.y >= height)
            {
                return;
            }

            tileStatus[hulkPos.x, hulkPos.y] = true;
            tileViews[hulkPos.x, hulkPos.y].SpriteRenderer.color = Color.white;
            brokenTiles.Add(hulkPos);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            
            GenerateHulkDrunkard(brokenTiles.Count == 0?
                new Vector2Int(width/2, height/2):
                brokenTiles[Random.Range(0, brokenTiles.Count)]);
        }
    }
}

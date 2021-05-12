using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Voronoi : MonoBehaviour
{
    struct VoronoiTile
    {
        public Vector2Int position;
        public Vector2 worldPos;
        public int region;
    }

    class Region
    {
        private List<VoronoiTile> tiles = new List<VoronoiTile>();
        
        private Color color = Color.grey;

        private Vector2 position = Vector2.zero;
        
        public List<VoronoiTile> Tiles => tiles;

        public Color Color
        {
            get => color;
            set => color = value;
        }

        public Vector2 Position
        {
            get => position;
            set => position = value;
        }

        public Vector2 CalculateCenter()
        {
            Vector2 center = Vector2Int.zero;
            foreach (var tile in tiles)
            {
                center += tile.worldPos;
            }

            return center/tiles.Count;
        }
    }

    
    [SerializeField] private Tile tilePrefab;
    private VoronoiTile[,] tiles;
    private Tile[,] tileViews;
    private Region[] regions;
    [SerializeField] private int width = 0;
    [SerializeField] private int height = 0;
    [SerializeField] private int regionNmb = 10;
    [SerializeField] private int seed = 0;
    [SerializeField] private bool showcase = false;
    [SerializeField] private int floydIteration = 5;


    // Start is called before the first frame update
    void Start()
    { 
        var mainCamera = Camera.main;
        var cameraSize = 2.0f * mainCamera.orthographicSize * new Vector2(mainCamera.aspect, 1.0f);
        var cameraRect = new Rect(){min=-cameraSize/2.0f, max = cameraSize/2.0f};
        regions = new Region[regionNmb];
        
        System.Random pseudoRandom = new System.Random(seed);
        Random.InitState(seed);
        for (int i = 0; i < regions.Length; i++)
        {
            regions[i] = new Region();
            var pos = new Vector2(Random.Range(cameraRect.xMin, cameraRect.xMax),
                Random.Range(cameraRect.yMin, cameraRect.yMax));
            var color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
            regions[i].Position = pos;
            regions[i].Color = color;
        }
        tiles = new VoronoiTile[width, height];
        tileViews = new Tile[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var vTile = new VoronoiTile();
                vTile.position = new Vector2Int(x, y);
                vTile.worldPos = new Vector2(x-width / 2, y-height / 2)+Vector2.one*0.5f;
                tiles[x, y] = vTile;
                tileViews[x, y] = Instantiate(tilePrefab, 
                    new Vector3(vTile.worldPos.x, vTile.worldPos.y), 
                    Quaternion.identity, transform);
            }
        }
        FillRegions();
        if (!showcase)
        {
            for (int i = 0; i < floydIteration; i++)
            {
                Floyd();
            }
        }
    }

    void FillRegions()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var tilePos = tiles[x,y].worldPos;
                float closestDistance = -1.0f;
                Region closestRegion = null;
                foreach (var region in regions)
                {
                    var delta = (region.Position - tilePos);
                    var distance = Mathf.Abs(delta.x) + Mathf.Abs(delta.y);
                    if (distance < closestDistance || closestDistance < 0.0f)
                    {
                        closestRegion = region;
                        closestDistance = distance;
                    }
                }

                var tile = tiles[x, y];
                tile.region = Array.IndexOf(regions, closestRegion);
                tiles[x, y] = tile;
                closestRegion.Tiles.Add(tiles[x,y]);

                tileViews[x, y].SpriteRenderer.color = closestRegion.Color;
            }
        }
    }

    void Floyd()
    {
        foreach (var region in regions)
        {
            region.Position = region.CalculateCenter();
            region.Tiles.Clear();
        }
        FillRegions();
    }

    // Update is called once per frame
    void Update()
    {
        if (showcase)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Floyd();
            }
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellularAutomata : MonoBehaviour
{
    [SerializeField] private CellBehavior cellBehaviorPrefab;

    private CellBehavior[,] cellViews;
    private Cell[,] cells;
    private Cell[,] previousCells;
    

    [SerializeField] private int width = 0;
    [SerializeField] private int height = 0;
    private const float cellSize = 0.1f;
    [SerializeField] private int aliveToDeathConversion = 4;
    [SerializeField] private int deathToAliveConversion = 4;

    [SerializeField] private bool showcase = false;
    [SerializeField] private int seed = 0;
    [Range(0.0f,1.0f)][SerializeField] private double randomFillFactor = 0.5;
    // Start is called before the first frame update
    void Start()
    {
        cellViews = new CellBehavior[width, height];
        cells = new Cell[width, height];
        previousCells = new Cell[width, height];
        System.Random pseudoRandom = new System.Random(seed);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3((x-width/2)*cellSize,(y-height/2)*cellSize, 0.0f);
                var cell = Instantiate(cellBehaviorPrefab, position, Quaternion.identity, transform);
                if (x == 0 || y == 0 || x == width-1 || y == height-1)
                {
                    cell.IsAlive = false;
                }
                else
                {
                    cell.IsAlive = pseudoRandom.NextDouble() < randomFillFactor;   
                }
                cellViews[x, y] = cell;
                cells[x, y] = new Cell(cell.IsAlive);
            }
        }

        if (!showcase)
        {
            for (int i = 0; i < 5; i++)
            {
                Iterate();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (showcase && Input.GetMouseButtonDown(0))
        {
            Iterate();
        }
    }

    void CopyCellIntoPrevious()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                previousCells[x, y] = cells[x, y];
            }
        }
    }
    
    /// <summary>
    /// Make a Game of Life iteration
    /// </summary>
    void Iterate()
    {
        CopyCellIntoPrevious();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int aliveNeighborCount = GetAliveNeighborCount(x, y);
                bool isAlive = previousCells[x, y].isAlive;
                //Too many alive neighbor means death
                if (isAlive && aliveNeighborCount < aliveToDeathConversion)
                {
                    cells[x, y] = new Cell(false);
                    cellViews[x, y].IsAlive = false;
                }
                //Enough living space means living
                else if(!isAlive && aliveNeighborCount > deathToAliveConversion)
                {
                    cells[x, y] = new Cell(true);
                    cellViews[x, y].IsAlive = true;
                }
            }
        }
    }

    int GetAliveNeighborCount(int currentX, int currentY)
    {
        int aliveNeighborCount = 0;
        for (int x = currentX - 1; x <= currentX + 1; x++)
        {
            for (int y = currentY - 1; y <= currentY + 1; y++)
            {
                if(x == currentX && y == currentY) continue;
                if(x < 0 || y < 0 || x >= width || y >= height) continue;
                aliveNeighborCount += previousCells[x, y].isAlive ? 1 : 0;
            }
        }
        return aliveNeighborCount;
    }
}

using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int width = 3;
    public int height = 5;
    public GameObject cellPrefab;
    public Cell[,] grid;

    public int defenderCellsToPlace = 16;
    public int attackerCellsToPlace = 20;

    public int defenderCellsPlaced = 0;
    public int attackerCellsPlaced = 0;

    public static GridManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        CreateGrid();
    }

    void CreateGrid()
    {
        grid = new Cell[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int x_shift = x - 20;
                int y_shift = y - 20;
                GameObject cellObj = Instantiate(cellPrefab, new Vector2(x_shift, y_shift), Quaternion.identity, transform);
                cellObj.name = $"Cell {x},{y}";
                grid[x, y] = cellObj.GetComponent<Cell>();
                grid[x, y].SetPosition(x_shift, y_shift);
                grid[x, y].SetAlive(false);
            }
        }
    }

    public void Randomize()
    {
        foreach (Cell cell in grid)
        {
            cell.SetAlive(Random.value > 0.7f);
        }
    }

    public void WallChanger(bool status)
    {
        for (int y = 0; y < height; y++)
        {
            grid[width / 2, y].SetAlive(status);
        }
    }
 
    public void Clear()
    {
        foreach (Cell cell in grid)
        {
            cell.SetAlive(false);
        }
        SimulationManager.Instance.IsPaused = true;
        SimulationManager.Instance.ResetPlacingPhase();
    }

    public int CountDefenderCellsAlive()
    {
        int count = 0;
        foreach (var cell in grid)
        {
            if (cell.IsAlive && cell.Owner == CellOwner.Defender)
                count++;
        }
        return count;
    }

}

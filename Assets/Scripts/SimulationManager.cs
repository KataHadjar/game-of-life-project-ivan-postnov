using UnityEngine;
using System.Collections;

public enum GamePhase
{
    Basic,
    DefenderPlacing,
    AttackerPlacing,
    Simulating,
    GameProcessing,
    Finished
}

public enum GameResults
{
    None,
    Attacker,
    Defender
}

public class SimulationManager : MonoBehaviour
{
    public static SimulationManager Instance;
    public bool IsPaused = true;
    public float simulationSpeed = 0.2f;

    public GamePhase phase = GamePhase.DefenderPlacing;
    public GameResults results = GameResults.None;

    private Coroutine simulationCoroutine;

    public int maxSteps = 200;
    private int currentStep = 0;


    private void Awake()
    {
        Instance = this;
    }

    public void ToggleSimulation()
    {
        IsPaused = !IsPaused;
        if (!IsPaused)
        {
            simulationCoroutine = StartCoroutine(RunSimulation());
        }
        else if (simulationCoroutine != null)
        {
            StopCoroutine(simulationCoroutine);
        }
    }

    IEnumerator RunSimulation()
    {
        while (!IsPaused)
        {
            if (phase == GamePhase.GameProcessing)
            {
                if (GridManager.Instance.CountDefenderCellsAlive() == 0)
                {

                    GridManager.Instance.Clear();
                    results = GameResults.Attacker;
                } 
                else if (currentStep >= maxSteps)
                {
                    results = GameResults.Defender;
                    GridManager.Instance.Clear();
                } else
                {
                    currentStep++;
                    Step();
                    yield return new WaitForSeconds(simulationSpeed);
                }
                
            }
            else
            {
                Step();
                yield return new WaitForSeconds(simulationSpeed);
            }
                
        }
    }

    void Step()
    {
        Cell[,] oldGrid = GridManager.Instance.grid;
        int width = GridManager.Instance.width;
        int height = GridManager.Instance.height;
        bool[,] newStates = new bool[width, height];
        CellOwner[,] newOwners = new CellOwner[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int aliveNeighbors = CountAliveNeighbors(x, y);
                bool currentAlive = oldGrid[x, y].IsAlive;

                bool willLive = currentAlive switch
                {
                    true when (aliveNeighbors == 2 || aliveNeighbors == 3) => true,
                    false when aliveNeighbors == 3 => true,
                    _ => false,
                };

                newStates[x, y] = willLive;

                if (willLive)
                {
                    newOwners[x, y] = oldGrid[x, y].Owner;
                }
            }
        }

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                oldGrid[x, y].SetAlive(newStates[x, y], newOwners[x, y]);
    }


    int CountAliveNeighbors(int x, int y)
    {
        int count = 0;
        Cell[,] grid = GridManager.Instance.grid;
        int width = GridManager.Instance.width;
        int height = GridManager.Instance.height;

        for (int dx = -1; dx <= 1; dx++)
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;
                int nx = x + dx, ny = y + dy;
                if (nx >= 0 && ny >= 0 && nx < width && ny < height)
                    if (grid[nx, ny].IsAlive)
                        count++;
            }

        return count;
    }

    public void SetSpeed(float value)
    {
        simulationSpeed = value;
    }

    public void ResetPlacingPhase()
    {
        GridManager.Instance.defenderCellsPlaced = 0;
        GridManager.Instance.attackerCellsPlaced = 0;
        phase = GamePhase.Basic;
    }
}

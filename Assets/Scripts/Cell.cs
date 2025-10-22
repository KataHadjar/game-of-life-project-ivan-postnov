using UnityEngine;

public enum CellOwner
{
    None,
    Defender,
    Attacker
}

public class Cell : MonoBehaviour
{
    public bool IsAlive = false;
    private int Xpos;
    private int Ypos;
    private SpriteRenderer spriteRenderer;
    public CellOwner Owner = CellOwner.None;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetAlive(bool alive, CellOwner owner = CellOwner.None)
    {
        IsAlive = alive;
        Owner = alive ? owner : CellOwner.None;

        switch (Owner)
        {
            case CellOwner.Defender:
                spriteRenderer.color = Color.blue;
                break;
            case CellOwner.Attacker:
                spriteRenderer.color = Color.red;
                break;
            default:
                spriteRenderer.color = alive ? Color.black : Color.white;
                break;
        }
    }

    public void SetPosition(int x, int y)
    {
        Xpos = x; Ypos = y;
    }

    bool IsInsideDefenseZone()
    {
        int w = GridManager.Instance.width;
        return Xpos > w / 2 - 20;
    }

    public void OnClicked()
    {
        if (SimulationManager.Instance.phase == GamePhase.DefenderPlacing)
        {
            if (IsInsideDefenseZone() && !IsAlive &&
                GridManager.Instance.defenderCellsPlaced < GridManager.Instance.defenderCellsToPlace)
            {
                SetAlive(true, CellOwner.Defender);
                GridManager.Instance.defenderCellsPlaced++;

                if (GridManager.Instance.defenderCellsPlaced >= GridManager.Instance.defenderCellsToPlace)
                {
                    SimulationManager.Instance.phase = GamePhase.AttackerPlacing;
                }
            }
        }
        else if (SimulationManager.Instance.phase == GamePhase.AttackerPlacing)
        {
            if (!IsInsideDefenseZone() && !IsAlive &&
                GridManager.Instance.attackerCellsPlaced < GridManager.Instance.attackerCellsToPlace)
            {
                SetAlive(true, CellOwner.Attacker);
                GridManager.Instance.attackerCellsPlaced++;

                if (GridManager.Instance.attackerCellsPlaced >= GridManager.Instance.attackerCellsToPlace)
                {
                    SimulationManager.Instance.phase = GamePhase.GameProcessing;
                    GridManager.Instance.WallChanger(false);
                    SimulationManager.Instance.ToggleSimulation();
                }
            }
        }
        else
        {
            if (SimulationManager.Instance.IsPaused)
            {
                SetAlive(!IsAlive);
            }
        }
    }

}

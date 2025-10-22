using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UIManager : MonoBehaviour
{
    public Button startPauseButton;
    public Button clearButton;
    public Button randomButton;
    public Button gameButton;
    public Button surrenderButton;


    public Slider speedSlider;

    public TMP_Text startPauseButtonText;
    public TMP_Text phaseText;
    public TMP_Text defenderCountText;
    public TMP_Text attackerCountText;
    public TMP_Text defenderAliveText;
    public TMP_Text lastGameResult;


    private void Start()
    {
        startPauseButton.onClick.AddListener(ToggleSimulation);
        clearButton.onClick.AddListener(() =>
        {
            GridManager.Instance.Clear();
            SimulationManager.Instance.ResetPlacingPhase();
        });
        randomButton.onClick.AddListener(() =>
        {
            GridManager.Instance.Clear();
            GridManager.Instance.Randomize();
            SimulationManager.Instance.phase = GamePhase.Simulating;
        });

        gameButton.onClick.AddListener(() =>
        {
            GridManager.Instance.Clear();
            SimulationManager.Instance.phase = GamePhase.DefenderPlacing;
            GridManager.Instance.WallChanger(true);
        });

        speedSlider.onValueChanged.AddListener((v) =>
        {
            SimulationManager.Instance.SetSpeed(v);
        });

        surrenderButton.onClick.AddListener(Surrender);
    }

    private void Update()
    {
        UpdatePhaseText();
        UpdateResultText();
        UpdateCounts();
        UpdateStartPauseText();

        surrenderButton.gameObject.SetActive(
            SimulationManager.Instance.phase == GamePhase.DefenderPlacing ||
            SimulationManager.Instance.phase == GamePhase.AttackerPlacing
        );

        defenderAliveText.gameObject.SetActive(
            SimulationManager.Instance.phase == GamePhase.DefenderPlacing ||
            SimulationManager.Instance.phase == GamePhase.AttackerPlacing
        );

        defenderCountText.gameObject.SetActive(
            SimulationManager.Instance.phase == GamePhase.DefenderPlacing ||
            SimulationManager.Instance.phase == GamePhase.AttackerPlacing
        );

        attackerCountText.gameObject.SetActive(
            SimulationManager.Instance.phase == GamePhase.DefenderPlacing ||
            SimulationManager.Instance.phase == GamePhase.AttackerPlacing
        );

    }

    void ToggleSimulation()
    {
        SimulationManager.Instance.ToggleSimulation();
        startPauseButtonText.text = SimulationManager.Instance.IsPaused ? "�����" : "�����";
    }

    void UpdateStartPauseText()
    {
        startPauseButtonText.text = SimulationManager.Instance.IsPaused ? "�����" : "�����";
    }
    void UpdatePhaseText()
    {
        string phase = SimulationManager.Instance.phase switch
        {
            GamePhase.DefenderPlacing => "����: ��������",
            GamePhase.AttackerPlacing => "����: ����������",
            GamePhase.Simulating => "����: ���������",
            GamePhase.Finished => "����: ���������",
            GamePhase.Basic => "����: ������������ �����",
            GamePhase.GameProcessing => "����: ��� ��������!",
            _ => "���� ����������"
        };

        phaseText.text = phase;
    }

    void UpdateResultText()
    {
        string result = SimulationManager.Instance.results switch
        {
            GameResults.None => "�� ����� ���� �� ������",
            GameResults.Attacker => "������� ���������!",
            GameResults.Defender => "������� ��������!",
            _ => "���� ����������"
        };

        lastGameResult.text = result;
    }

    void UpdateCounts()
    {
        int defLeft = GridManager.Instance.defenderCellsToPlace - GridManager.Instance.defenderCellsPlaced;
        int attLeft = GridManager.Instance.attackerCellsToPlace - GridManager.Instance.attackerCellsPlaced;
        int defAlive = GridManager.Instance.CountDefenderCellsAlive();

        defenderCountText.text = $"�������� ����������: {Mathf.Max(defLeft, 0)}";
        attackerCountText.text = $"�������� ���������: {Mathf.Max(attLeft, 0)}";
        defenderAliveText.text = $"����� ����������: {defAlive}";
    }

    void Surrender()
    {
        if (SimulationManager.Instance.phase == GamePhase.DefenderPlacing)
        {
            GridManager.Instance.Clear();
            SimulationManager.Instance.results = GameResults.Attacker;
        }
        else if (SimulationManager.Instance.phase == GamePhase.AttackerPlacing)
        {
            GridManager.Instance.Clear();
            SimulationManager.Instance.results = GameResults.Defender;
        }
        else
        {
            return;
        }
    }

}

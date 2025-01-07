using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveUIManager : MonoBehaviour
{
    public static WaveUIManager Instance { get; private set; }

    [Header("UI Elements")]
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI turnText;
    public TextMeshProUGUI enemiesRemainingText;
    public TextMeshProUGUI winTurnsRemainingText;
    public GameObject SoftWinPanel;
    public Button EndNowButton;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        EndNowButton.onClick.AddListener(() => WaveManager.Instance.FinishCurrentWaveNow());
    }

    public void UpdateWaveUI()
    {
        if (WaveManager.Instance != null)
        {
            waveText.text = $"Wave: {WaveManager.Instance.currentWaveIndex + 1} / {WaveManager.Instance.MaxWaveIndex + 1}";
            turnText.text = $"Turn: {WaveManager.Instance.CurrentTurn} / {WaveManager.Instance.CurrentMaxTurn}";
            enemiesRemainingText.text = $"Enemies Remaining: {WaveManager.Instance.GetRemainingEnemies()}";
            winTurnsRemainingText.text = $"End Encounter in {WaveManager.Instance.CurrentMaxTurn + 1 - WaveManager.Instance.CurrentTurn} turn(s)";
        }
        
    }
}

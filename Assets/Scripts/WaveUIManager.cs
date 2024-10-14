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
    }

    private void Start()
    {
        UpdateWaveUI();
    }

    public void UpdateWaveUI()
    {
        if (WaveManager.Instance != null)
        {
            waveText.text = $"Wave: {WaveManager.Instance.currentWaveIndex + 1}";
            turnText.text = $"Turn: {WaveManager.Instance.CurrentTurn}";
            enemiesRemainingText.text = $"Enemies Remaining: {WaveManager.Instance.GetRemainingEnemies()}";
        }
    }
}

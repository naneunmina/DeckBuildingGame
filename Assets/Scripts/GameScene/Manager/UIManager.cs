using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text turnText;
    [SerializeField] private Slider timerSlider;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text almondText;
    [SerializeField] private TMP_Text sugarText;
    [SerializeField] private TMP_Text eggText;
    [SerializeField] private TMP_Text scoreText;

    [Header("Managers")]
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private ShopManager shopManager;

    void OnEnable()
    {
        turnManager.OnTurnChanged.AddListener(UpdateTurnUI);
        turnManager.OnTimerUpdated.AddListener(UpdateTimerUI);
        turnManager.OnGoldChanged.AddListener(UpdateGoldUI);
        scoreManager.OnScoreChanged.AddListener(UpdateScoreUI);
        resourceManager.OnResourceChanged.AddListener(UpdateResourcesUI);
    }

    void Start()
    {
        // 초기값 표시
        UpdateTurnUI(turnManager.currentTurn);
        UpdateGoldUI(turnManager.currentGold);
        UpdateResourcesUI();
        UpdateScoreUI(scoreManager.GetFinalScore());
        // 매 턴 종료 때도 리소스 갱신
        turnManager.OnTurnEnded.AddListener(UpdateResourcesUI);
    }

    void UpdateTurnUI(int turn)
    {
        turnText.text = $"Turn {turn}/{turnManager.maxTurns}";
        if (turn>1) shopManager.RefreshShop();
    }

    void UpdateTimerUI(float remaining)
    {
        timerSlider.value = remaining / turnManager.turnTimeSeconds;
        timerText.text = $"{Mathf.CeilToInt(remaining)}s";
    }

    void UpdateGoldUI(int gold)
    {
        goldText.text = $"{gold}G";
    }

    void UpdateResourcesUI()
    {
        var rm = resourceManager;
        almondText.text = rm.almond.ToString();
        sugarText.text = rm.sugar.ToString();
        eggText.text = rm.egg.ToString();
    }
    
    public void UpdateScoreUI(int score)
    {
        scoreText.text = $"Score: {score}";
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TMP_Text turnText;
    public Slider timerSlider;
    public TMP_Text timerText;
    public TMP_Text goldText;
    public TMP_Text almondText;
    public TMP_Text sugarText;
    public TMP_Text eggText;

    public TurnManager turnManager;
    public ResourceManager resourceManager;

    void OnEnable()
    {
        turnManager.OnTurnChanged.AddListener(UpdateTurnUI);
        turnManager.OnTimerUpdated.AddListener(UpdateTimerUI);
        turnManager.OnGoldChanged.AddListener(UpdateGoldUI);
    }

    void Start()
    {
        // 초기값 표시
        UpdateTurnUI(turnManager.currentTurn);
        UpdateGoldUI(turnManager.currentGold);
        UpdateResourcesUI();
        // 매 턴 종료 때도 리소스 갱신
        turnManager.OnTurnEnded.AddListener(UpdateResourcesUI);
    }

    void UpdateTurnUI(int turn)
    {
        turnText.text = $"Turn {turn}/{turnManager.maxTurns}";
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
        sugarText.text  = rm.sugar.ToString();
        eggText.text    = rm.egg.ToString();
    }
}

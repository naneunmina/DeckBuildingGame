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
    [SerializeField] private TMP_Text plainCountText;
    [SerializeField] private TMP_Text[] specialCountTexts;

    [Header("Facility Rate UI")]
    [SerializeField] private TMP_Text almondRateText;
    [SerializeField] private TMP_Text sugarRateText;
    [SerializeField] private TMP_Text eggRateText;
    [SerializeField] private TMP_Text macaronRateText;

    [Header("Managers")]
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private ShopManager shopManager;
    [SerializeField] private MacaronManager macaronManager;
    [SerializeField] private FacilityManager facilityManager;

    void OnEnable()
    {
        turnManager.OnTurnChanged.AddListener(UpdateTurnUI);
        turnManager.OnTimerUpdated.AddListener(UpdateTimerUI);
        turnManager.OnGoldChanged.AddListener(UpdateGoldUI);
        scoreManager.OnScoreChanged.AddListener(UpdateScoreUI);
        resourceManager.OnResourceChanged.AddListener(UpdateResourcesUI);
        facilityManager.OnAlmondProductionChanged.AddListener(UpdateAlmondRateUI);
        facilityManager.OnSugarProductionChanged.AddListener(UpdateSugarRateUI);
        facilityManager.OnEggProductionChanged.AddListener(UpdateEggRateUI);
        facilityManager.OnMacaronProductionChanged.AddListener(UpdateMacaronRateUI);
        macaronManager.OnPlainCountChanged.AddListener(UpdatePlainCountUI);
        macaronManager.OnSpecialCountChanged.AddListener(UpdateSpecialCountUI);
    }

    void Start()
    {
        // 초기값 표시
        UpdateTurnUI(turnManager.currentTurn);
        UpdateGoldUI(turnManager.currentGold);
        UpdateResourcesUI();
        UpdateScoreUI(scoreManager.GetFinalScore());
        UpdatePlainCountUI(macaronManager.GetPlainCount());
        UpdateAlmondRateUI(facilityManager.AlmondProduction);
        UpdateSugarRateUI(facilityManager.SugarProduction);
        UpdateEggRateUI(facilityManager.EggProduction);
        UpdateMacaronRateUI(facilityManager.MacaronProduction);
        foreach (SpecialType t in System.Enum.GetValues(typeof(SpecialType)))
        {
            UpdateSpecialCountUI(t, macaronManager.GetSpecialCount(t));
        }

        // 매 턴 종료 때도 리소스 갱신
        turnManager.OnTurnEnded.AddListener(UpdateResourcesUI);
    }

    private void UpdateTurnUI(int turn)
    {
        turnText.text = $"Turn {turn}/{turnManager.maxTurns}";
        if (turn > 1) shopManager.RefreshShop();
    }

    private void UpdateTimerUI(float remaining)
    {
        timerSlider.value = remaining / turnManager.turnTimeSeconds;
        timerText.text = $"{Mathf.CeilToInt(remaining)}s";
    }

    private void UpdateGoldUI(int gold)
    {
        goldText.text = $"{gold}G";
    }

    private void UpdateResourcesUI()
    {
        var rm = resourceManager;
        almondText.text = rm.almond.ToString();
        sugarText.text = rm.sugar.ToString();
        eggText.text = rm.egg.ToString();
    }

    private void UpdateScoreUI(int score)
    {
        scoreText.text = $"Score: {score}";
    }
    private void UpdatePlainCountUI(int count)
    {
        plainCountText.text = $"{count}";
    }

    public void UpdateSpecialCountUI(SpecialType type, int count)
    {
        int idx = (int)type;
        if (idx >= 0 && idx < specialCountTexts.Length)
        {
            specialCountTexts[idx].text = $"{count}";
        }
    }
    
    private void UpdateAlmondRateUI(int rate)   => almondRateText.text   = $"+{rate}";
    private void UpdateSugarRateUI(int rate)    => sugarRateText.text    = $"+{rate}";
    private void UpdateEggRateUI(int rate)      => eggRateText.text      = $"+{rate}";
    private void UpdateMacaronRateUI(int rate)  => macaronRateText.text  = $"+{rate}";
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurnClockUI : MonoBehaviour
{
    [SerializeField] private TurnManager turnManager;

    [Header("UI")]
    [SerializeField] private Image dial;       // 시계판(옵션)
    [SerializeField] private Image fill;       // 라디얼 채움 (Filled-Radial)

    [Header("비주얼 옵션")]
    [SerializeField] private Gradient fillColorOverTime;   // 1→0로 갈수록 색 변화
    [SerializeField] private Color handColor = Color.white;
    [SerializeField] private float criticalSeconds = 10f;  // 임박 시 효과용

    private float lastNormalized = 1f;

    void OnEnable()
    {
        if (turnManager == null) return;
        turnManager.OnTimerUpdated.AddListener(OnTimer);
        turnManager.OnTurnChanged.AddListener(OnTurnStart);
    }
    void OnDisable()
    {
        if (turnManager == null) return;
        turnManager.OnTimerUpdated.RemoveListener(OnTimer);
        turnManager.OnTurnChanged.RemoveListener(OnTurnStart);
    }

    void OnTurnStart(int turn)
    {
        UpdateClock(turnManager.turnTimeSeconds);
    }

    void OnTimer(float remaining)
    {
        UpdateClock(remaining);
    }

    void UpdateClock(float remaining)
    {
        float total = Mathf.Max(0.0001f, turnManager.turnTimeSeconds);
        float normalized = Mathf.Clamp01(remaining / total);        // 1→0
        float elapsedNorm = 1f - normalized;                        // 0→1

        // 1) 라디얼 채움
        if (fill != null)
        {
            fill.fillAmount = normalized;                           // 꽉 찬 상태에서 줄어드는 방식
            if (fillColorOverTime.colorKeys.Length > 0)
                fill.color = fillColorOverTime.Evaluate(elapsedNorm);
        }

        lastNormalized = normalized;
    }
}

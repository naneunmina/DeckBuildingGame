using UnityEngine;

public class DayCyclePerTurn : MonoBehaviour
{
    public enum Phase { Morning, Day, Evening, Night }

    [Header("Sprites")]
    [SerializeField] Sprite morning;
    [SerializeField] Sprite day;
    [SerializeField] Sprite evening;
    [SerializeField] Sprite night;

    [Header("Renderers (아래/위)")]
    [SerializeField] SpriteRenderer lowerRenderer; // 현재 페이즈 (불투명)
    [SerializeField] SpriteRenderer upperRenderer; // 다음 페이즈 (알파 페이드)

    [Header("Refs")]
    [SerializeField] TurnManager turnManager;

    [Header("각 구간 길이(초) — 합은 자동으로 턴 길이에 맞춰 스케일됨")]
    [SerializeField] float morningSeconds = 15f;
    [SerializeField] float daySeconds     = 40f;
    [SerializeField] float eveningSeconds = 20f;
    [SerializeField] float nightSeconds   = 15f;

    [Header("페이드 커브")]
    [SerializeField] AnimationCurve blendCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    Sprite[] sprites;
    float[] segmentDur;    // 스케일된 실제 초
    float[] cumulativeEnd; // 누적 끝시각(초)
    float cycleDuration;   // = turnManager.turnTimeSeconds
    int currentIndex = -1;

    const float eps = 0.0001f;

    void Awake()
    {
        sprites = new[] { morning, day, evening, night };
    }

    void OnEnable()
    {
        if (turnManager == null) return;
        turnManager.OnTurnChanged.AddListener(OnTurnStarted);
        turnManager.OnTimerUpdated.AddListener(OnTimerUpdated);
    }

    void OnDisable()
    {
        if (turnManager == null) return;
        turnManager.OnTurnChanged.RemoveListener(OnTurnStarted);
        turnManager.OnTimerUpdated.RemoveListener(OnTimerUpdated);
    }

    void OnTurnStarted(int _)
    {
        RebuildSchedule();
        // 시작 즉시 아침으로 세팅
        EnterSegment(0, 1);
        SetUpperAlpha(0f);
        OnTimerUpdated(turnManager.turnTimeSeconds);
    }

    void OnTimerUpdated(float remaining)
    {
        if (turnManager == null || cycleDuration <= 0f) return;

        // 0→cycleDuration, 밤에서 멈추도록 끝을 살짝 클램프
        float elapsed = Mathf.Clamp(cycleDuration - remaining, 0f, cycleDuration - eps);
        UpdateBlend(elapsed);
    }

    void RebuildSchedule()
    {
        cycleDuration = Mathf.Max(eps, turnManager.turnTimeSeconds);

        // 입력 합
        float sum = Mathf.Max(eps, morningSeconds + daySeconds + eveningSeconds + nightSeconds);
        float scale = cycleDuration / sum;

        segmentDur = new float[4];
        segmentDur[0] = Mathf.Max(eps, morningSeconds * scale);
        segmentDur[1] = Mathf.Max(eps, daySeconds     * scale);
        segmentDur[2] = Mathf.Max(eps, eveningSeconds * scale);
        segmentDur[3] = Mathf.Max(eps, nightSeconds   * scale);

        cumulativeEnd = new float[4];
        float acc = 0f;
        for (int i = 0; i < 4; i++)
        {
            acc += segmentDur[i];
            cumulativeEnd[i] = acc;
        }
        cumulativeEnd[3] = cycleDuration; // 마지막은 정확히 턴 길이로 스냅

        currentIndex = -1;
    }

    void UpdateBlend(float elapsedSec)
    {
        int idx = 0;
        float startTime = 0f;

        // 현재 세그먼트 찾기
        for (int i = 0; i < 4; i++)
        {
            if (elapsedSec <= cumulativeEnd[i])
            {
                idx = i;
                startTime = (i == 0) ? 0f : cumulativeEnd[i - 1];
                break;
            }
        }

        // 마지막 세그먼트에서는 다음을 자기 자신으로 고정(밤에서 멈춤)
        int next = (idx == 3) ? 3 : idx + 1;

        if (idx != currentIndex)
        {
            EnterSegment(idx, next);
            SetUpperAlpha(0f);
            currentIndex = idx;
        }

        float dur = Mathf.Max(eps, segmentDur[idx]);
        float localT = Mathf.Clamp01((elapsedSec - startTime) / dur);

        float a = blendCurve.Evaluate(localT);
        if (localT <= 0.001f) a = 0f;
        else if (localT >= 0.999f) a = 1f;

        SetUpperAlpha(a);
    }

    void EnterSegment(int idx, int next)
    {
        var baseSprite = sprites[idx];
        var nextSprite = sprites[next];

        lowerRenderer.sprite = baseSprite;
        upperRenderer.sprite = nextSprite;

#if UNITY_EDITOR
        if (baseSprite == null || nextSprite == null)
            Debug.LogWarning($"[DayCyclePerTurn] Missing sprite at idx={idx} or next={next}");
#endif

        var lc = lowerRenderer.color; lc.a = 1f; lowerRenderer.color = lc;
    }

    void SetUpperAlpha(float a)
    {
        var c = upperRenderer.color;
        c.a = Mathf.Clamp01(a);
        upperRenderer.color = c;
    }
}

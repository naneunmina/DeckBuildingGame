// TimeOfDayBackground.cs
using UnityEngine;
#if USING_URP_2D
using UnityEngine.Experimental.Rendering.Universal; // Global Light 2D 쓰면
#endif

public class TimeOfDayBackground : MonoBehaviour
{
    public enum Phase { Morning, Day, Evening, Night }

    [Header("Sprites (아침/낮/저녁/밤)")]
    [SerializeField] private Sprite morning;
    [SerializeField] private Sprite day;
    [SerializeField] private Sprite evening;
    [SerializeField] private Sprite night;

    [Header("Renderers (위/아래)")]
    [SerializeField] private SpriteRenderer lowerRenderer; // 바닥
    [SerializeField] private SpriteRenderer upperRenderer; // 위에 얹어 페이드

    [Header("연결")]
    [SerializeField] private TurnManager turnManager;

    [Header("페이드 커브(선형이면 Linear)")]
    [SerializeField] private AnimationCurve blendCurve = AnimationCurve.Linear(0, 0, 1, 1);

#if USING_URP_2D
    [Header("옵션: Global Light 2D")]
    [SerializeField] private Light2D globalLight;
    [SerializeField] private Gradient lightColorOverDay;  // t:[0..1] 색
    [SerializeField] private AnimationCurve lightIntensityOverDay = AnimationCurve.Linear(0, 1, 1, 0.6f);
#endif

    private Sprite[] sprites;
    private int currentIdx = -1;
    private void Awake()
  {
    sprites = new[] { morning, day, evening, night };
    EnterSegment(Phase.Morning, Phase.Day); // 초기 세팅
    SetUpperAlpha(0f);
  }

    private void OnEnable()
    {
        if (turnManager != null)
        {
            // 남은 시간(초) 이벤트를 이용해 진행도 계산
            turnManager.OnTimerUpdated.AddListener(HandleTimer);
            turnManager.OnTurnChanged.AddListener(_ => RefreshAtTurnStart());
        }
    }
    private void OnDisable()
    {
        if (turnManager != null)
        {
            turnManager.OnTimerUpdated.RemoveListener(HandleTimer);
            turnManager.OnTurnChanged.RemoveListener(_ => RefreshAtTurnStart());
        }
    }

    private void RefreshAtTurnStart()
    {
        // 턴 시작 시 즉시 올바른 페이즈로 맞춤
        HandleTimer(turnManager.turnTimeSeconds);
    }

    private void HandleTimer(float remainingSeconds)
    {
        if (turnManager == null || turnManager.turnTimeSeconds <= 0f) return;

        // 0..1 : 현재 진행도
        float turnT = 1f - Mathf.Clamp01(remainingSeconds / turnManager.turnTimeSeconds);

        float t = turnT % 1f;

        UpdateBlend(t);
    }

    private void UpdateBlend(float t01)
    {
        const int count = 4;
        float segLen = 1f / count;

        int idx = Mathf.FloorToInt(t01 / segLen);
        if (idx >= count) idx = count - 1;

        int next = (idx + 1) % count;
        float localT = (t01 - idx * segLen) / segLen;

        // 세그먼트가 바뀐 순간에만 스프라이트 교체 (한 프레임 팝 제거)
        if (idx != currentIdx)
        {
            EnterSegment((Phase)idx, (Phase)next);
            SetUpperAlpha(0f);
            currentIdx = idx;
        }

        // 알파는 매 프레임 갱신하되, 경계에서 하드 클램프로 끊김 방지
        float a = blendCurve != null ? blendCurve.Evaluate(localT) : localT;
        if (localT <= 0.001f) a = 0f;
        else if (localT >= 0.999f) a = 1f;

        SetUpperAlpha(a);
    }

    private void EnterSegment(Phase basePhase, Phase nextPhase)
    {
        var baseIdx = (int)basePhase;
        var nextIdx = (int)nextPhase;

        var baseSprite = sprites != null && baseIdx < sprites.Length ? sprites[baseIdx] : null;
        var nextSprite = sprites != null && nextIdx < sprites.Length ? sprites[nextIdx] : null;

        if (baseSprite == null || nextSprite == null)
        {
            Debug.LogWarning($"[TimeOfDayBackground] Missing sprite: base={basePhase}({baseSprite}), next={nextPhase}({nextSprite})");
        }

        lowerRenderer.sprite = baseSprite;
        upperRenderer.sprite = nextSprite;

        // 하단은 항상 불투명
        var lc = lowerRenderer.color; lc.a = 1f; lowerRenderer.color = lc;
    }

    private void SetUpperAlpha(float a)
    {
        var uc = upperRenderer.color;
        uc.a = Mathf.Clamp01(a);
        upperRenderer.color = uc;
    }
}

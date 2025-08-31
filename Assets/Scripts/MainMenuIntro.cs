using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuIntro : MonoBehaviour
{
    [Header("Refs")]
    public RectTransform viewport;     // 화면 전체
    public RectTransform panImage;     // 배경 (왼쪽-중앙 정렬)
    public CanvasGroup logoGroup;      // 로고 묶음
    public CanvasGroup menuGroup;      // 버튼 묶음
    public RectTransform car;          // 자동차(UI 이미지)

    [Header("Layout")]
    public bool useCover = true;               // 꽉 채우기(크롭) / 전체 보이기(여백)
    [Range(1f, 2f)] public float extraWidth = 1.15f;
    [Range(-1f, 1f)] public float verticalBias = 0f;

    [Header("Pan")]
    public float panSeconds = 6f;              // 배경 오른쪽으로 훑는 시간
    public AnimationCurve ease = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("UI Appear")]
    public float uiDelay = 0.1f;               // 팬 완료 후 텀
    public float uiFadeSeconds = 0f;           // 0이면 즉시 등장

    [Header("Car Motion (Squash & Stretch)")]
    public bool carMotion = true;
    [Range(0f, 1f)] public float carStartAt = 0.3f; // 팬 진행률 기준 시작 시점
    public float carFrequencyHz = 1.2f;
    [Range(0f, 0.6f)] public float carStretch = 0.15f;
    public bool carConserveVolume = true;
    public bool carBob = true;
    public float carBobPixels = 8f;
    public float carBobFrequencyHz = 1.2f;

    [Header("Car Stop Options")]
    public bool carStopAtPanEnd = true;        // 팬 끝나면 멈추기
    public bool carSnapNeutralOnStop = true;   // 멈출 때 중립값으로 스냅(스케일/위치)
    public float carSettleSeconds = 0.2f;      // 0보다 크면 부드럽게 정렬(스냅 대신)

    [Header("Skip")]
    public bool enableTapBoost = true;         // 탭 가속
    public float boostMultiplier = 2.5f;
    public bool enableHoldSkip = true;         // 길게 눌러 스킵
    public float holdSkipSeconds = 0.6f;

    // --- internals ---
    Vector2 startPos, endPos;
    bool fastForward;
    float holdTimer;

    bool carActive;
    Vector3 carBaseScale;
    Vector2 carBasePos;
    float carPhase;
    Coroutine settleCo;

    void Start()
    {
        SetupGroup(logoGroup, 0f, false);
        SetupGroup(menuGroup, 0f, false);

        ApplyLayout();
        CalcPan();

        if (car)
        {
            carBaseScale = car.localScale;
            carBasePos = car.anchoredPosition;
            carPhase = Random.Range(0f, Mathf.PI * 2f);
        }

        StartCoroutine(PlayIntro());
    }

    void Update()
    {
        if (enableTapBoost && (Input.GetMouseButtonDown(0) || Input.touchCount > 0 || Input.anyKeyDown))
            fastForward = true;

        if (enableHoldSkip && (Input.GetMouseButton(0) || Input.touchCount > 0))
        {
            holdTimer += Time.unscaledDeltaTime;
            if (holdTimer >= holdSkipSeconds) SkipAll();
        }
        else holdTimer = 0f;

        if (carMotion && carActive && car) UpdateCarMotion(Time.unscaledTime);
    }

    IEnumerator PlayIntro()
    {
        float t = 0f;
        while (t < panSeconds)
        {
            float dt = Time.unscaledDeltaTime * (fastForward ? boostMultiplier : 1f);
            t += dt; fastForward = false;

            float p = Mathf.Clamp01(t / panSeconds);
            panImage.anchoredPosition = Vector2.LerpUnclamped(startPos, endPos, ease.Evaluate(p));

            if (carMotion && !carActive && p >= carStartAt) carActive = true;
            yield return null;
        }

        // === 여기서 자동차 정지 ===
        if (carStopAtPanEnd) StopCarMotion(carSnapNeutralOnStop);

        if (uiDelay > 0f) yield return new WaitForSecondsRealtime(uiDelay);
        yield return ShowUi();
    }

    void SkipAll()
    {
        StopAllCoroutines();
        panImage.anchoredPosition = endPos;

        // 스킵 시에도 자동차 정지(즉시 중립으로 정렬 권장)
        if (carMotion) StopCarMotion(true);

        StartCoroutine(ShowUi());
    }

    IEnumerator ShowUi()
    {
        if (uiFadeSeconds <= 0f)
        {
            SetupGroup(logoGroup, 1f, true);
            SetupGroup(menuGroup, 1f, true);
            yield break;
        }
        yield return FadeGroup(logoGroup, 0f, 1f, uiFadeSeconds);
        yield return FadeGroup(menuGroup, 0f, 1f, uiFadeSeconds);
    }

    // ----- car motion -----
    void UpdateCarMotion(float nowUnscaled)
    {
        float s = Mathf.Sin(2f * Mathf.PI * carFrequencyHz * nowUnscaled + carPhase) * carStretch;
        float scaleY = Mathf.Max(0.001f, 1f + s);
        float scaleX = carConserveVolume ? (1f / scaleY) : (1f - s * 0.6f);
        scaleX = Mathf.Max(0.001f, scaleX);
        car.localScale = new Vector3(carBaseScale.x * scaleX, carBaseScale.y * scaleY, carBaseScale.z);

        if (carBob)
        {
            float by = Mathf.Sin(2f * Mathf.PI * carBobFrequencyHz * nowUnscaled + carPhase) * carBobPixels;
            car.anchoredPosition = new Vector2(carBasePos.x, carBasePos.y + by);
        }
    }

    void StopCarMotion(bool snapNeutral)
    {
        carActive = false;
        if (!car) return;

        if (settleCo != null) StopCoroutine(settleCo);

        if (snapNeutral)
        {
            // 즉시 중립값
            car.localScale = carBaseScale;
            car.anchoredPosition = carBasePos;
        }
        else if (carSettleSeconds > 0f)
        {
            settleCo = StartCoroutine(SettleCarToNeutral(carSettleSeconds));
        }
        // snapNeutral=false && carSettleSeconds<=0 → 현재 자세에 그대로 정지
    }

    IEnumerator SettleCarToNeutral(float seconds)
    {
        Vector3 fromScale = car.localScale;
        Vector2 fromPos = car.anchoredPosition;
        float t = 0f;
        while (t < seconds)
        {
            t += Time.unscaledDeltaTime;
            float k = t / seconds;
            car.localScale = Vector3.Lerp(fromScale, carBaseScale, k);
            car.anchoredPosition = Vector2.Lerp(fromPos, carBasePos, k);
            yield return null;
        }
        car.localScale = carBaseScale;
        car.anchoredPosition = carBasePos;
        settleCo = null;
    }

    // ----- layout helpers -----
    void ApplyLayout()
    {
        float imgW = 1248f, imgH = 832f;
        var raw = panImage.GetComponent<RawImage>();
        if (raw && raw.texture) { imgW = raw.texture.width; imgH = raw.texture.height; }
        var img = panImage.GetComponent<Image>();
        if (img && img.sprite && img.sprite.texture) { imgW = img.sprite.texture.width; imgH = img.sprite.texture.height; }

        float viewW = Mathf.Max(1f, viewport.rect.width);
        float viewH = Mathf.Max(1f, viewport.rect.height);

        float scaleToW = viewW / imgW;
        float scaleToH = viewH / imgH;
        float scale = useCover ? Mathf.Max(scaleToW, scaleToH) : Mathf.Min(scaleToW, scaleToH);

        float targetW = imgW * scale;
        float targetH = imgH * scale;
        if (useCover) targetW *= extraWidth;

        var rt = panImage;
        rt.anchorMin = new Vector2(0, 0.5f);
        rt.anchorMax = new Vector2(0, 0.5f);
        rt.pivot = new Vector2(0, 0.5f);
        rt.sizeDelta = new Vector2(targetW, targetH);

        float overflowY = Mathf.Max(0f, targetH - viewH);
        float bias01 = Mathf.InverseLerp(-1f, 1f, verticalBias);
        float y = Mathf.Lerp(0f, -overflowY, bias01);
        rt.anchoredPosition = new Vector2(0f, y);
    }

    void CalcPan()
    {
        float viewW = viewport.rect.width;
        float panW = panImage.rect.width;
        float dist = Mathf.Max(0f, panW - viewW);
        startPos = new Vector2(0f, panImage.anchoredPosition.y);
        endPos = new Vector2(-dist, panImage.anchoredPosition.y);
    }

    void SetupGroup(CanvasGroup cg, float alpha, bool interactable)
    {
        if (!cg) return;
        cg.alpha = alpha;
        cg.interactable = interactable;
        cg.blocksRaycasts = interactable;
        cg.gameObject.SetActive(true);
    }

    IEnumerator FadeGroup(CanvasGroup cg, float from, float to, float seconds)
    {
        if (!cg) yield break;
        cg.gameObject.SetActive(true);
        cg.interactable = false; cg.blocksRaycasts = false;

        float t = 0f;
        while (t < seconds)
        {
            t += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(from, to, t / seconds);
            yield return null;
        }
        cg.alpha = to;
        cg.interactable = true; cg.blocksRaycasts = true;
    }

    void OnRectTransformDimensionsChange()
    {
        ApplyLayout();
        CalcPan();
    }
    
    public void LoadGame()
    {
        Debug.Log($"[UI] Start clicked. timeScale={Time.timeScale}");
        // 1) 어떤 대기/코루틴보다 먼저 정상속도로
        Time.timeScale = 1f;

        // 2) 싱글톤 있으면 사용, 없으면 바로 로드
        if (SceneLoader.i != null) SceneLoader.i.LoadGame();
        else SceneManager.LoadScene("GameScene");
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class HowToUI : MonoBehaviour
{
    [Header("Refs")]
    public GameObject root;          // HowToRoot
    public CanvasGroup panelGroup;   // Panel에 붙인 CanvasGroup (없어도 동작)
    public GameObject firstFocus;    // 열릴 때 포커스 줄 UI (선택)

    [Header("Behavior")]
    public bool showOnLoad = true;   // GameScene 로드시 자동 표시
    public bool pauseOnOpen = true;  // 열 때 일시정지
    public bool onlyFirstTime = false; // "한 번만 보기" 모드
    public string prefsKey = "howto_seen_v1";
    public float fadeSeconds = 0f;   // 0이면 페이드 없이 즉시

    float prevTimeScale = 1f;
    bool isOpen;

    void Start()
    {
        if (showOnLoad && !(onlyFirstTime && PlayerPrefs.GetInt(prefsKey, 0) == 1))
            Open();
        else if (pauseOnOpen)
            Time.timeScale = 1f; // 혹시 모를 잔여 정지 복구
    }

    public void Open()
    {
        if (isOpen) return;
        if (pauseOnOpen) { prevTimeScale = Time.timeScale; Time.timeScale = 0f; }
        isOpen = true;

        root.SetActive(true);
        if (panelGroup) StartCoroutine(Fade(panelGroup, panelGroup.alpha, 1f, fadeSeconds));
        if (firstFocus) UnityEngine.EventSystems.EventSystem.current?.SetSelectedGameObject(firstFocus);
    }

    public void CloseAndStart()
    {
        if (!isOpen) return;
        isOpen = false;

        if (onlyFirstTime) { PlayerPrefs.SetInt(prefsKey, 1); PlayerPrefs.Save(); }

        if (panelGroup && fadeSeconds > 0f)
            StartCoroutine(Fade(panelGroup, panelGroup.alpha, 0f, fadeSeconds, AfterClose));
        else
            AfterClose();
    }

    void AfterClose()
    {
        if (panelGroup) panelGroup.alpha = 0f;
        root.SetActive(false);
        if (pauseOnOpen) Time.timeScale = Mathf.Max(1f, prevTimeScale);
    }

    System.Collections.IEnumerator Fade(CanvasGroup cg, float from, float to, float s, System.Action done = null)
    {
        if (!cg || s <= 0f) { if (cg) cg.alpha = to; done?.Invoke(); yield break; }
        float t = 0f;
        while (t < s)
        {
            t += Time.unscaledDeltaTime; // 일시정지 중에도 부드럽게
            cg.alpha = Mathf.Lerp(from, to, t / s);
            yield return null;
        }
        cg.alpha = to; done?.Invoke();
    }
}

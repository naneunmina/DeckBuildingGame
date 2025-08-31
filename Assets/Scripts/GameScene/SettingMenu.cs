using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("Refs")]
    public GameObject settingsRoot;      // Backdrop + SettingsPanel 묶음
    public CanvasGroup panelGroup;       // SettingsPanel에 CanvasGroup 추가해서 연결(없으면 null 가능)
    public Button settingsButton;        // "설정" 버튼(옵션: 코드에서 리스너 달아줌)
    public GameObject firstFocus;        // 열릴 때 포커스 줄 UI(슬라이더/토글 등)

    [Header("Behavior")]
    public bool pauseOnOpen = true;      // 열 때 게임 일시정지
    public KeyCode toggleKey = KeyCode.Escape;
    public float fadeSeconds = 0.15f;    // 0이면 즉시

    float prevTimeScale = 1f;
    bool isOpen;

    void Awake()
    {
        SetActive(false, immediate:true);
        if (settingsButton) settingsButton.onClick.AddListener(OpenSettings);
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            if (isOpen) CloseSettings();
            else        OpenSettings();
        }
    }

    public void OpenSettings()
    {
        if (isOpen) return;
        if (pauseOnOpen) { prevTimeScale = Time.timeScale; Time.timeScale = 0f; }

        SetActive(true, immediate:false);
        isOpen = true;

        // 첫 포커스(키보드/패드 사용 시 편함)
        if (firstFocus) UnityEngine.EventSystems.EventSystem.current?.SetSelectedGameObject(firstFocus);
    }

    public void CloseSettings()
    {
        if (!isOpen) return;
        if (pauseOnOpen) Time.timeScale = prevTimeScale;

        SetActive(false, immediate:false);
        isOpen = false;

        // 닫힌 뒤 폴더블 클릭 방지용 포커스 해제
        UnityEngine.EventSystems.EventSystem.current?.SetSelectedGameObject(null);
    }

    public void ToggleSettings()
    {
        if (isOpen) CloseSettings();
        else OpenSettings();
    }

    void SetActive(bool show, bool immediate)
    {
        if (!settingsRoot) return;

        settingsRoot.SetActive(true); // 페이드처리를 위해 일단 켜둠

        if (panelGroup && !immediate && fadeSeconds > 0f)
        {
            StopAllCoroutines();
            StartCoroutine(Fade(panelGroup, show ? 0f : 1f, show ? 1f : 0f, fadeSeconds, onEnd: () =>
            {
                settingsRoot.SetActive(show);
            }));
        }
        else
        {
            if (panelGroup) panelGroup.alpha = show ? 1f : 0f;
            settingsRoot.SetActive(show);
        }
    }

    IEnumerator Fade(CanvasGroup cg, float from, float to, float dur, System.Action onEnd)
    {
        float t = 0f;
        cg.alpha = from;
        // 패널 열릴 때만 입력 막기
        cg.blocksRaycasts = true;
        cg.interactable = false;

        while (t < dur)
        {
            t += Time.unscaledDeltaTime; // 일시정지 중에도 부드럽게
            cg.alpha = Mathf.Lerp(from, to, t/dur);
            yield return null;
        }
        cg.alpha = to;

        // 열렸다면 클릭 가능, 닫혔다면 입력 차단 해제
        bool open = to > 0.99f;
        cg.interactable = open;
        cg.blocksRaycasts = open;

        onEnd?.Invoke();
    }
    public void ReloadActive()
    {
        Time.timeScale = 1f;
        if (SceneLoader.i != null) SceneLoader.i.ReloadActive();
        else SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        if (SceneLoader.i != null) SceneLoader.i.LoadMenu();
        else SceneManager.LoadScene("MainMenu");
    }
}

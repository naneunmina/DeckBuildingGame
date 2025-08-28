using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreboardUI : MonoBehaviour
{
    public static ScoreboardUI i;

    [SerializeField] ScoreManager scoreManager;

    [Header("Refs")]
    public CanvasGroup root;          // ScoreboardRoot의 CanvasGroup
    public TextMeshProUGUI txtScore;

    [Header("Anim")]
    public float fadeSeconds = 0.15f;   // 0이면 즉시
    public float countSeconds = 0.6f;   // 스코어 카운트업 시간

    [Header("Behavior")]
    public bool pauseOnOpen = true;

    float prevTimeScale = 1f;
    bool isOpen;

    void Awake()
    {
        i = this;
        if (root)
        {
            root.alpha = 0f;
            root.interactable = false;
            root.blocksRaycasts = false;
            root.gameObject.SetActive(false);
        }
    }

    public void Open()
    {
        if (isOpen) return;
        isOpen = true;

        if (pauseOnOpen) { prevTimeScale = Time.timeScale; Time.timeScale = 0f; }

        root.gameObject.SetActive(true);
        StartCoroutine(ShowRoutine(scoreManager.GetFinalScore()));
    }

    IEnumerator ShowRoutine(int r)
    {
        yield return Fade(root, 0f, 1f, fadeSeconds);

        // 숫자 표시
        if (txtScore)   yield return CountUp(txtScore, 0, r, countSeconds);

        root.interactable = true;
        root.blocksRaycasts = true;
    }

    IEnumerator Fade(CanvasGroup cg, float from, float to, float dur)
    {
        if (!cg || dur <= 0f) { if (cg) cg.alpha = to; yield break; }
        float t = 0f;
        cg.alpha = from;
        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(from, to, t / dur);
            yield return null;
        }
        cg.alpha = to;
    }

    IEnumerator CountUp(TextMeshProUGUI target, int from, int to, float dur)
    {
        if (!target) yield break;
        float t = 0f;
        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            int v = (int)Mathf.Lerp(from, to, Mathf.SmoothStep(0, 1, t / dur));
            target.text = v.ToString("N0");
            yield return null;
        }
        target.text = to.ToString("N0");
    }

    public void OnClickRestart()
    {
        if (pauseOnOpen) Time.timeScale = Mathf.Max(1f, prevTimeScale);
        if (SceneLoader.i != null) SceneLoader.i.ReloadActive();
        else
        {
            var s = SceneManager.GetActiveScene();
            SceneManager.LoadScene(s.buildIndex);
        }
    }

    public void OnClickMainMenu()
    {
        if (pauseOnOpen) Time.timeScale = Mathf.Max(1f, prevTimeScale);
        if (SceneLoader.i != null) SceneLoader.i.LoadMenu();
        else SceneManager.LoadScene("MainMenu");
    }
}

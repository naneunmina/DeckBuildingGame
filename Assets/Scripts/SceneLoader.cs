using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader i { get; private set; }
    bool isLoading;

    [Header("Optional")]
    public float clickSfxDelay = 0.05f; // 클릭 사운드가 살짝이라도 나오게(원하면 0으로)

    void Awake()
    {
        if (i != null && i != this) { Destroy(gameObject); return; }
        i = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadMenu()     => StartCoroutine(Load("MainMenu"));
    public void LoadGame()     => StartCoroutine(Load("GameScene"));
    public void ReloadActive() => StartCoroutine(Load(SceneManager.GetActiveScene().name));

    IEnumerator Load(string sceneName)
    {
        if (isLoading) yield break;
        isLoading = true;

        Time.timeScale = 1f; // 전환 전 항상 정상속도로
        if (clickSfxDelay > 0f)
            yield return new WaitForSecondsRealtime(clickSfxDelay);

        var op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone) yield return null;

        isLoading = false;
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EventResultPopup : MonoBehaviour
{
    public CanvasGroup root;
    public Image imgResult;
    public TMP_Text txtResult;
    public Button btnOk;

    public bool pauseDuring = true;

    float prevScale;
    System.Action onClosed;

    void Awake()
    {
        HideImmediate();
        if (btnOk) btnOk.onClick.AddListener(Close);
    }

    public void Show(Sprite art, string text, System.Action closed = null)
    {
        if (pauseDuring) { prevScale = Time.timeScale; Time.timeScale = 0f; }
        onClosed = closed;

        if (imgResult) { imgResult.enabled = (art != null); imgResult.sprite = art; }
        if (txtResult) txtResult.text = text ?? "";

        root.gameObject.SetActive(true);
        root.alpha = 1f; root.interactable = true; root.blocksRaycasts = true;
    }

    public void Close()
    {
        HideImmediate();
        onClosed?.Invoke();
        onClosed = null;
    }

    void HideImmediate()
    {
        if (!root) return;
        root.alpha = 0f; root.interactable = false; root.blocksRaycasts = false;
        root.gameObject.SetActive(false);
        if (pauseDuring) Time.timeScale = (prevScale > 0f ? prevScale : 1f);
    }
}

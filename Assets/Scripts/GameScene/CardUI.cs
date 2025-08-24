using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class CardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] RectTransform root;
    [SerializeField] private Image icon;
    [SerializeField] private Image background;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text discriptionText;
    [SerializeField] private GameObject costUI;
    [SerializeField] private TMP_Text costText;

    [Header("Hover Animation")]
    [SerializeField] float animTime = 0.15f;
    [SerializeField] AnimationCurve ease = null; // 기본 null이면 내부에서 EaseInOut
    [SerializeField] float hoverScale = 1.06f;   // 카드 확대

    private CardInstance instance;
    private HandManager handManager;
    private ShopManager shopManager;
    private ResourceManager resourceManager;
    private TurnManager turnManager;
    private bool inShop;
    private int slotIndex;

    Coroutine hoverRoutine;

    public void Initialize(
        CardInstance data,
        HandManager hm,
        ResourceManager rm,
        ShopManager sm,
        TurnManager tm,
        bool isInShop,
        int index)
    {
        instance = data;
        handManager = hm;
        resourceManager = rm;
        shopManager = sm;
        turnManager = tm;
        inShop = isInShop;
        slotIndex = index;

        icon.sprite = data.Data.icon;
        background.sprite = data.Data.background;
        nameText.text = data.Data.cardName;
        discriptionText.text = data.Data.cardDiscription;
        costText.text = data.Data.GetCost().ToString();

        var btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        costUI.gameObject.SetActive(inShop);
        if (inShop)
            btn.onClick.AddListener(() => shopManager.Purchase(slotIndex));
        else
            btn.onClick.AddListener(() => handManager.UseCard(instance));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!inShop) return;
        StartHover(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!inShop) return;
        StartHover(false);
    }

    void StartHover(bool enter)
    {
        if (hoverRoutine != null) StopCoroutine(hoverRoutine);
        hoverRoutine = StartCoroutine(AnimateHover(enter));
    }

    IEnumerator AnimateHover(bool forward)
    {
        float t = 0f;
        float duration = Mathf.Max(0.01f, animTime);

        Vector3 fromRoot = root ? root.localScale : Vector3.one;
        Vector3 toRoot   = forward ? Vector3.one * hoverScale : Vector3.one;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime; // 게임 일시정지와 무관하게 UI 애니메이션
            float k = ease.Evaluate(Mathf.Clamp01(t / duration));

            if (root) root.localScale = Vector3.LerpUnclamped(fromRoot, toRoot, k);
            yield return null;
        }

        // 최종값 스냅
        if (root) root.localScale = toRoot;

        hoverRoutine = null;
    }
}
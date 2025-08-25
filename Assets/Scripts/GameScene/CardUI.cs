using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class CardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] RectTransform root;
    [SerializeField] private Image icon;
    [SerializeField] private Image background;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text discriptionText;
    [SerializeField] private GameObject costUI;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private Sprite bronzeBack;
    [SerializeField] private Sprite silverBack;
    [SerializeField] private Sprite goldBack;

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
    Coroutine hoverAnim;

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

        StopAllCoroutines();

        icon.sprite = data.Data.icon[0];
        if (data.Data.cardRarity == CardRarity.Bronze)
        {
            background.sprite = bronzeBack;
        }
        else if (data.Data.cardRarity == CardRarity.Silver)
        {
            background.sprite = silverBack;
        }
        else
        {
            background.sprite = goldBack;
        }
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
        if (inShop) StartHandAnim();
        StartHover(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (inShop) StopHandAnim();
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
        Vector3 toRoot = forward ? Vector3.one * hoverScale : Vector3.one;

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
    
    void StartHandAnim()
    {
        if (instance.Data.icon.Count < 2)
            return;
        hoverAnim = StartCoroutine(HandAnimLoop());
    }

    void StopHandAnim()
    {
        if (hoverAnim != null) { StopCoroutine(hoverAnim); hoverAnim = null; }
        // 첫 프레임으로 복구
        icon.sprite = instance.Data.icon[0];
    }

    IEnumerator HandAnimLoop()
    {
        float interval = 1f / Mathf.Max(1f, 6f);

        int i = 0;

        while (true)
        {
            icon.sprite = instance.Data.icon[i];

            i++;
            if (i >= instance.Data.icon.Count)
            {
                i = 0;
            }

            // UI 애니메이션은 unscaledDeltaTime 권장
            float t = 0f;
            while (t < interval) { t += Time.unscaledDeltaTime; yield return null; }
        }
    }
}
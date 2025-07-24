using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text costText;
    private CardSO cardData;
    private HandManager handMgr;
    private ShopManager shopMgr;
    private TurnManager turnMgr;
    private ResourceManager resourceManager;  // ← 추가
    private int slotIndex;
    private bool isInShop;

    // Initialize 시 ResourceManager rm 인자도 받도록 시그니처 변경
    public void Initialize(
        CardSO data,
        HandManager hm,
        ResourceManager rm,     // ← 추가
        ShopManager sm,
        TurnManager tm,
        bool inShop,
        int slotIdx)
    {
        cardData        = data;
        handMgr         = hm;
        shopMgr         = sm;
        turnMgr         = tm;
        resourceManager = rm;   // ← 저장
        isInShop        = inShop;
        slotIndex       = slotIdx;

        icon.sprite     = data.icon;
        costText.text   = data.cost.ToString();

        var btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        if (isInShop)
            btn.onClick.AddListener(() => shopMgr.Purchase(slotIndex, handMgr, turnMgr));
        else
            btn.onClick.AddListener(() => handMgr.UseCard(cardData, turnMgr, resourceManager, shopMgr));
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text costText;

    private CardSO cardData;
    private HandManager handManager;
    private ShopManager shopManager;
    private ResourceManager resourceManager;
    private TurnManager turnManager;
    private bool inShop;
    private int slotIndex;

    public void Initialize(
        CardSO data,
        HandManager hm,
        ResourceManager rm,
        ShopManager sm,
        TurnManager tm,
        bool isInShop,
        int index)
    {
        cardData = data;
        handManager = hm;
        resourceManager = rm;
        shopManager = sm;
        turnManager = tm;
        inShop = isInShop;
        slotIndex = index;

        icon.sprite = data.icon;
        nameText.text = data.cardName;
        costText.text = data.cost.ToString();

        var btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        if (inShop)
            btn.onClick.AddListener(() => shopManager.Purchase(slotIndex));
        else
            btn.onClick.AddListener(() => handManager.UseCard(cardData));
    }
}
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text costText;

    private CardInstance instance;
    private HandManager handManager;
    private ShopManager shopManager;
    private ResourceManager resourceManager;
    private TurnManager turnManager;
    private bool inShop;
    private int slotIndex;

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
        nameText.text = data.Data.cardName;
        costText.text = data.Data.cost.ToString();

        var btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        if (inShop)
            btn.onClick.AddListener(() => shopManager.Purchase(slotIndex));
        else
            btn.onClick.AddListener(() => handManager.UseCard(instance));
    }
}
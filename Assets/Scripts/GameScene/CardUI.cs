using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Image background;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text discriptionText;
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
        background.sprite = data.Data.background;
        nameText.text = data.Data.cardName;
        discriptionText.text = data.Data.cardDiscription;
        costText.text = data.Data.GetCost().ToString();

        var btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        if (inShop)
            btn.onClick.AddListener(() => shopManager.Purchase(slotIndex));
        else
            btn.onClick.AddListener(() => handManager.UseCard(instance));
    }
}
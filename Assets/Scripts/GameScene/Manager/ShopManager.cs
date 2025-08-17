using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShopManager : MonoBehaviour
{
    [Header("Shop Settings")]
    [SerializeField] private int shopSize = 5;
    [SerializeField] private int lockedSlots = 2;
    [SerializeField, Range(0f, 1f)] private float silverChance = 0.08f;
    [SerializeField] private List<CardSO> bronzePool = new List<CardSO>();
    [SerializeField] private List<CardSO> silverPool = new List<CardSO>();
    [SerializeField] private List<CardSO> goldPool = new List<CardSO>();
    [SerializeField] private int refreshCost = 1;

    [Header("UI References")]
    [SerializeField] private Transform[] shopSlotParents;
    [SerializeField] private CardUI cardUiPrefab;
    [SerializeField] private HandManager handManager;
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private TurnManager turnManager;

    private List<CardInstance> shopSlots;
    private List<bool> isLocked;

    private int forcedDropsRemaining = 0;
    private CardRarity forcedRarity = CardRarity.Bronze;

    public UnityEvent<List<CardInstance>, List<bool>> OnShopChanged;

    private void Start()
    {
        shopSlots = new List<CardInstance>(shopSize);
        isLocked = new List<bool>(shopSize);
        for (int i = 0; i < shopSize; i++)
        {
            shopSlots.Add(null);
            isLocked.Add(i < lockedSlots);
        }
        RefreshShop();
    }

    public void SetSilverDropChance(float probability)
    {
        silverChance = Mathf.Clamp01(probability);
    }

    public void ForceNextDropsSilver(int count)
    {
        forcedRarity = CardRarity.Silver;
        forcedDropsRemaining = Mathf.Max(forcedDropsRemaining, Mathf.Max(0, count));
    }

    public void ForceNextDropsGold(int count)
    {
        forcedRarity = CardRarity.Gold;
        forcedDropsRemaining = Mathf.Max(forcedDropsRemaining, Mathf.Max(0, count));
    }

    public void ClearForcedDrops()
    {
        forcedDropsRemaining = 0;
    }

    public void RefreshShop()
    {
        for (int i = 0; i < shopSize; i++)
            shopSlots[i] = RollCardFromPools();
        RefreshShopUI();
    }

    public void RerollSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= shopSlots.Count) return;
        shopSlots[slotIndex] = RollCardFromPools();
        RefreshShopUI();
    }

    public void RefreshShopWithCost()
    {
        if (!turnManager.SpendGold(refreshCost)) return;
        RefreshShop();
    }

    private void RefreshShopUI()
    {
        for (int i = 0; i < shopSlotParents.Length; i++)
        {
            foreach (Transform child in shopSlotParents[i]) Destroy(child.gameObject);
            if (isLocked[i]) continue;
            var card = shopSlots[i];
            if (card != null)
            {
                var ui = Instantiate(cardUiPrefab, shopSlotParents[i]);
                ui.Initialize(
                    card,
                    handManager,
                    resourceManager,
                    this,
                    turnManager,
                    true,
                    i
                );
            }
        }
        OnShopChanged?.Invoke(shopSlots, isLocked);
    }

    public bool Purchase(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= shopSize) return false;
        if (isLocked[slotIndex]) return false;
        var card = shopSlots[slotIndex];
        if (card == null) return false;
        if (!turnManager.SpendGold(card.Data.GetCost())) return false;

        bool drawn = handManager.DrawCard(card);
        if (!drawn)
        {
            // Hand is full: auto-play card without extra cost
            card.Data.Play(turnManager, resourceManager, handManager, this);
        }
        shopSlots[slotIndex] = RollCardFromPools();
        RefreshShopUI();
        return true;
    }

    public bool UnlockSlot(int slotIndex, int unlockCost)
    {
        if (slotIndex < 0 || slotIndex >= shopSize) return false;
        if (!isLocked[slotIndex]) return false;
        if (!turnManager.SpendGold(unlockCost)) return false;

        isLocked[slotIndex] = false;
        RefreshShop();
        return true;
    }
    
    private CardInstance RollCardFromPools()
    {
        CardRarity rarity = RollRarity();
        CardSO data = DrawFromPool(rarity);

        return new CardInstance(data);
    }

    private CardRarity RollRarity()
    {
        // Forced drops take precedence
        if (forcedDropsRemaining > 0)
        {
            forcedDropsRemaining--;
            return forcedRarity;
        }

        // Otherwise, only Silver vs Bronze are random
        return (Random.value < silverChance) ? CardRarity.Silver : CardRarity.Bronze;
    }

    private CardSO DrawFromPool(CardRarity r)
    {
        List<CardSO> pool = r == CardRarity.Bronze ? bronzePool : r == CardRarity.Silver ? silverPool : goldPool;
        if (pool == null || pool.Count == 0) return null;
        return pool[Random.Range(0, pool.Count)];
    }
}
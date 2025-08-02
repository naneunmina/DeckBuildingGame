using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShopManager : MonoBehaviour
{
    [Header("Shop Settings")]
    [SerializeField] private int shopSize = 5;
    [SerializeField] private int lockedSlots = 2;
    [SerializeField] private List<CardSO> cardPool;
    [SerializeField] private int refreshCost = 1;

    [Header("UI References")]
    [SerializeField] private Transform[] shopSlotParents;
    [SerializeField] private CardUI cardUiPrefab;
    [SerializeField] private HandManager handManager;
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private TurnManager turnManager;

    private List<CardInstance> shopSlots;
    private List<bool> isLocked;

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

    public void RefreshShop()
    {
        for (int i = 0; i < shopSize; i++)
        {
            if (isLocked[i]) continue;
            CardSO data = cardPool[Random.Range(0, cardPool.Count)];
            shopSlots[i] = new CardInstance(data);
        }
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
        shopSlots[slotIndex] = new CardInstance(
            cardPool[Random.Range(0, cardPool.Count)]
        );
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
}
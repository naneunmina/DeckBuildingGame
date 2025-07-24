// Assets/Scripts/Managers/ShopManager.cs
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

public class ShopManager : MonoBehaviour
{
    public int shopSize = 5;
    public int lockedSlots = 2;
    public List<CardSO> shopSlots = new List<CardSO>();
    public List<bool> isLocked = new List<bool>();

    /// <summary>상점 슬롯이 바뀔 때 UI 갱신용 이벤트</summary>
    public UnityEvent<List<CardSO>, List<bool>> OnShopChanged;

    public List<CardSO> cardPool;  // 에디터에서 할당: 모든 구매 가능한 카드 목록

    private void Start()
    {
        // 슬롯 초기화
        for (int i = 0; i < shopSize; i++)
        {
            shopSlots.Add(null);
            isLocked.Add(i < lockedSlots);
        }
        RefreshShop();
    }

    /// <summary>상점 새로고침: 잠긴 슬롯 제외</summary>
    public void RefreshShop()
    {
        var available = cardPool.OrderBy(_ => Random.value).ToList();
        int idx = 0;
        for (int i = 0; i < shopSize; i++)
        {
            if (isLocked[i]) continue;
            shopSlots[i] = available[idx++];
        }
        OnShopChanged?.Invoke(shopSlots, isLocked);
    }

    /// <summary>해당 슬롯 구매 시 호출</summary>
    public bool Purchase(int slotIndex, HandManager handManager, TurnManager turnManager)
    {
        if (slotIndex < 0 || slotIndex >= shopSize) return false;
        if (isLocked[slotIndex]) return false;

        var card = shopSlots[slotIndex];
        if (card == null) return false;
        if (!turnManager.SpendGold(card.cost)) return false;

        bool added = handManager.DrawCard(card);
        if (added)
        {
            shopSlots[slotIndex] = null;    // 슬롯 비움
            OnShopChanged?.Invoke(shopSlots, isLocked);
        }
        return added;
    }

    /// <summary>개별 슬롯 잠금 해제</summary>
    public bool UnlockSlot(int slotIndex, TurnManager turnManager, int unlockCost)
    {
        if (slotIndex < 0 || slotIndex >= shopSize) return false;
        if (!isLocked[slotIndex]) return false;
        if (!turnManager.SpendGold(unlockCost)) return false;

        isLocked[slotIndex] = false;
        RefreshShop();
        return true;
    }
}

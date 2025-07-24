// Assets/Scripts/Managers/HandManager.cs
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class HandManager : MonoBehaviour
{
    public int maxHandSize = 3;
    public List<CardSO> hand = new List<CardSO>();

    /// <summary>핸드가 바뀔 때 UI 갱신용 이벤트</summary>
    public UnityEvent<List<CardSO>> OnHandChanged;

    /// <summary>카드 풀에서 핸드로 카드 추가</summary>
    public bool DrawCard(CardSO card)
    {
        if (hand.Count >= maxHandSize) return false;
        hand.Add(card);
        OnHandChanged?.Invoke(hand);
        return true;
    }

    /// <summary>핸드에서 카드 사용</summary>
    public bool UseCard(CardSO card, 
                        TurnManager turnManager,
                        ResourceManager resourceManager,
                        ShopManager shopManager)
    {
        if (!hand.Contains(card)) return false;
        if (!turnManager.SpendGold(card.cost)) return false;

        hand.Remove(card);
        card.Play(turnManager, resourceManager, this, shopManager);

        OnHandChanged?.Invoke(hand);
        return true;
    }
}

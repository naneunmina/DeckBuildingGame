using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HandManager : MonoBehaviour
{
    [Header("Hand Settings")]
    [SerializeField] private int maxHandSize = 3;
    [SerializeField] private Transform[] handSlotParents;
    [SerializeField] private CardUI cardUiPrefab;
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private ShopManager shopManager;
    [SerializeField] private TurnManager turnManager;

    private readonly List<CardSO> hand = new List<CardSO>();
    public UnityEvent<List<CardSO>> OnHandChanged;

    private void Start()
    {
        OnHandChanged.AddListener(RefreshHandUI);
    }

    public bool DrawCard(CardSO card)
    {
        if (hand.Count >= maxHandSize) return false;
        hand.Add(card);
        OnHandChanged?.Invoke(hand);
        return true;
    }

    public bool UseCard(CardSO card)
    {
        if (!hand.Contains(card)) return false;

        hand.Remove(card);
        card.Play(turnManager, resourceManager, this, shopManager);
        OnHandChanged?.Invoke(hand);
        return true;
    }

    private void RefreshHandUI(List<CardSO> currentHand)
    {
        for (int i = 0; i < handSlotParents.Length; i++)
        {
            foreach (Transform child in handSlotParents[i]) Destroy(child.gameObject);
        }

        for (int i = 0; i < currentHand.Count; i++)
        {
            var slot = handSlotParents[i];
            var ui = Instantiate(cardUiPrefab, slot);
            ui.Initialize(
                currentHand[i],
                this,
                resourceManager,
                shopManager,
                turnManager,
                false,
                -1
            );
        }
    }
}
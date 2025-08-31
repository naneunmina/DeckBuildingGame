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

    private readonly List<CardInstance> hand = new();
    public UnityEvent<List<CardInstance>> OnHandChanged;
    public UnityEvent<CardInstance> OnCardUsed;

    private void Start()
    {
        OnHandChanged.AddListener(RefreshHandUI);
    }

    public bool DrawCard(CardInstance card)
    {
        if (hand.Count >= maxHandSize) return false;
        hand.Add(card);
        OnHandChanged?.Invoke(hand);
        return true;
    }

    public bool UseCard(CardInstance card)
    {
        if (!hand.Contains(card)) return false;

        hand.Remove(card);
        card.Data.Play(turnManager, resourceManager, this, shopManager);
        OnHandChanged?.Invoke(hand);
        return true;
    }

    private void RefreshHandUI(List<CardInstance> currentHand)
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
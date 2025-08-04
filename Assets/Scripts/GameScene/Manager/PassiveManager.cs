using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PassiveManager : MonoBehaviour
{
    [Header("Passive Pools")]
    public List<PassiveSO> startPassives;
    public List<PassiveSO> mid1Passives;
    public List<PassiveSO> mid2Passives;

    [Header("Events for UI")]
    public UnityEvent<PassiveCategory, List<PassiveSO>> OnPassiveOffer;
    public UnityEvent<PassiveSO> OnPassiveChosen;

    [Header("Game Managers")]
    public ResourceManager resourceManager;
    public FacilityManager facilityManager;
    public TurnManager turnManager;
    // public CustomerManager customerManager;
    // public DeckManager deckManager;
    public HandManager handManager;
    public ScoreManager scoreManager;

    // Currently active passives
    private readonly List<PassiveSO> activePassives = new List<PassiveSO>();

    private void Start()
    {
        // Offer the player a choice of start passives
        OfferPassives(PassiveCategory.Start);

        // Subscribe to game-wide events
        turnManager.OnTurnChanged.AddListener(HandleTurnStart);
        handManager.OnCardUsed.AddListener(HandleCardUsed);
    }

    private void OnDisable()
    {
        turnManager.OnTurnChanged.RemoveListener(HandleTurnStart);
        handManager.OnCardUsed.RemoveListener(HandleCardUsed);
    }

    /// <summary>
    /// Randomly selects 3 candidates from the pool and invokes the offer event.
    /// </summary>
    private void OfferPassives(PassiveCategory category)
    {
        List<PassiveSO> pool = GetPool(category);
        if (pool == null || pool.Count == 0) return;

        // Pick up to 3 random candidates
        var candidates = new List<PassiveSO>(pool);
        var offer = new List<PassiveSO>();
        for (int i = 0; i < 3 && candidates.Count > 0; i++)
        {
            int idx = Random.Range(0, candidates.Count);
            offer.Add(candidates[idx]);
            candidates.RemoveAt(idx);
        }

        // Notify UI
        OnPassiveOffer?.Invoke(category, offer);
        // Wait for selection
        OnPassiveChosen.AddListener(ApplyPassive);
    }

    /// <summary>
    /// Applies the chosen passive and stores it in active list.
    /// </summary>
    private void ApplyPassive(PassiveSO passive)
    {
        activePassives.Add(passive);
        passive.OnApply(this);
        OnPassiveChosen.RemoveListener(ApplyPassive);
    }

    /// <summary>
    /// Called each turn; dispatches OnTurnStart to active passives, and offers mid-passives at turns 5 and 9.
    /// </summary>
    private void HandleTurnStart(int turn)
    {
        foreach (var p in activePassives)
            p.OnTurnStart(this, turn);

        if (turn == 5) OfferPassives(PassiveCategory.Mid1);
        if (turn == 9) OfferPassives(PassiveCategory.Mid2);
    }

    /// <summary>
    /// Called when a card instance is used; dispatches OnCardUsed to active passives.
    /// </summary>
    private void HandleCardUsed(CardInstance cardInstance)
    {
        foreach (var p in activePassives)
            p.OnCardUsed(this, cardInstance);
    }

    /// <summary>
    /// Returns the pool list corresponding to a category.
    /// </summary>
    private List<PassiveSO> GetPool(PassiveCategory category)
    {
        switch (category)
        {
            case PassiveCategory.Start: return startPassives;
            case PassiveCategory.Mid1:  return mid1Passives;
            case PassiveCategory.Mid2:  return mid2Passives;
            default: return null;
        }
    }
}

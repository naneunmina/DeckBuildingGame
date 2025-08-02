using UnityEngine;

public enum CardType { Ingredient, Facility, Production, Enhancement, Event, Skill }

[CreateAssetMenu(fileName = "NewCard", menuName = "Deckaroon/Card")]
public class CardSO : ScriptableObject
{
    public string cardID;
    public string cardName;
    public CardType cardType;
    public int cost;
    public Sprite icon;
    // additional fields per card type
    public int value;

    /// <summary>
    /// Override in subclasses or via switch on cardType for effect.
    /// </summary>
    public virtual void Play(TurnManager turnManager,
                            ResourceManager resourceManager,
                            HandManager handManager,
                            ShopManager shopManager)
    {
        Debug.Log($"Playing card: {cardName}");
        // basic stub: implement behavior based on cardType or subclass
    }

    public virtual int GetCost() => cost;
}
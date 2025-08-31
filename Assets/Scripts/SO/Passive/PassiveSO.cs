using UnityEngine;

public enum PassiveCategory { Start, Mid1, Mid2 }

[CreateAssetMenu(fileName="PassiveSO", menuName="Deckaroon/PassiveSO")]
public abstract class PassiveSO : ScriptableObject
{
    [Header("Common")]
    public PassiveCategory category;
    [Tooltip("Only used for Mid1/Mid2")]
    public int level = 1;

    /// <summary>Called once, immediately when this passive is chosen.</summary>
    public virtual void OnApply(PassiveManager mgr) { }

    /// <summary>Called at the start of every turn.</summary>
    public virtual void OnTurnStart(PassiveManager mgr, int turn) { }

    /// <summary>Called whenever the player uses a card.</summary>
    public virtual void OnCardUsed(PassiveManager mgr, CardInstance cardInstance) { }
}
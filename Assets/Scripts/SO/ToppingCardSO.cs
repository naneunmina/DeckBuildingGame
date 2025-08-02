using NUnit.Framework.Internal.Commands;
using UnityEngine;

[CreateAssetMenu(fileName = "ToppingCard", menuName = "Deckaroon/ToppingCard")]
public class ToppingCardSO : CardSO
{
  [Header("Topping Settings")]
  [Tooltip("Level of this topping card (starts at 1)")]
  public int level = 1;

  [Tooltip("Type of special macaron to produce when played")]
  public SpecialType specialType;

  /// <summary>
  /// When played, produces 'level' special macarons by spending gold.
  /// Cost = baseCost * level.
  /// </summary>
  public override void Play(TurnManager turnManager,
                            ResourceManager resourceManager,
                            HandManager handManager,
                            ShopManager shopManager)
  {
    var macaronManager = Object.FindFirstObjectByType<MacaronManager>();
    if (macaronManager == null)
    {
      Debug.LogWarning("MacaronManager not found. Cannot produce special.");
      return;
    }

    int typeIndex = (int)specialType;
    macaronManager.ProduceSpecial(level, specialType);
  }

  public override int GetCost()
  {
    return base.GetCost()*level;
  }
}

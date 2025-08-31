// IngredientCardSO.cs
using UnityEngine;

[CreateAssetMenu(fileName = "New Ingredient Card", menuName = "Deckaroon/IngredientCard")]
public class IngredientCardSO : CardSO
{
  [Header("Ingredient Amounts per Use")]
  [Tooltip("Amount of almond flour to supply")]
  public int almondAmount = 0;
  [Tooltip("Amount of sugar to supply")]
  public int sugarAmount = 0;
  [Tooltip("Amount of eggs to supply")]
  public int eggAmount = 0;

  public override void Play(TurnManager turnManager,
                            ResourceManager resourceManager,
                            HandManager handManager,
                            ShopManager shopManager)
  {
    resourceManager.AddResource(almondAmount, sugarAmount, eggAmount);
  }
}

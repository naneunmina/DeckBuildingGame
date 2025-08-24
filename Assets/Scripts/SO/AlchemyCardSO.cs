using UnityEngine;

[CreateAssetMenu(fileName = "Enh_AlchemyI", menuName = "Deckaroon/Enhancement/Alchemy")]
public class AlchemyCardSO : CardSO
{
  [SerializeField] int nextCount;
  [SerializeField] CardRarity rarity;
  public override void Play(TurnManager turnManager,
                            ResourceManager resourceManager,
                            HandManager handManager,
                            ShopManager shopManager)
  {
    if (rarity == CardRarity.Silver)
    {
      shopManager.ForceNextDropsSilver(nextCount);
    }
    else if (rarity == CardRarity.Gold)
    {
      shopManager.ForceNextDropsGold(nextCount);
    }
  }
}

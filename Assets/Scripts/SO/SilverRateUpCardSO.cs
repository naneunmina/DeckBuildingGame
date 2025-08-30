using UnityEngine;

[CreateAssetMenu(fileName = "Enh_SilverIsBest", menuName = "Deckaroon/Enhancement/Silver")]
public class SilverRateUpCardSO : CardSO
{
  [SerializeField, Range(0f, 1f)] float addPercent;
    public override void Play(TurnManager turnManager,
                          ResourceManager resourceManager,
                          HandManager handManager,
                          ShopManager shopManager)
  {
    shopManager.SetSilverDropChance(addPercent);
  }
}

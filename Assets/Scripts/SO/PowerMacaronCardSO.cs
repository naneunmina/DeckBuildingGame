using UnityEngine;

[CreateAssetMenu(fileName = "PowerMacaronCardSO", menuName = "Deckaroon/Enhancement/PowerMacaronCardSO")]
public class PowerMacaronCardSO : CardSO
{
  [SerializeField] float bonus;
  public override void Play(TurnManager turnManager,
                            ResourceManager resourceManager,
                            HandManager handManager,
                            ShopManager shopManager)
  {
    var scoreManager = FindFirstObjectByType<ScoreManager>();
    scoreManager.SetBasePlainScore(scoreManager.GetBasePlainScore() + bonus);
  }
}

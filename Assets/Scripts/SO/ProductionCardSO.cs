using UnityEngine;

[CreateAssetMenu(fileName = "New Production Card", menuName = "Deckaroon/ProductionCardSO")]
public class ProductionCardSO : CardSO
{
    [Header("Production Settings")]
    [Tooltip("Number of plain macarons this card attempts to produce when played")]
    public int productionCount = 1;

    // Fixed recipe requirements per macaron
    private const int AlmondNeeded = 1;
    private const int SugarNeeded  = 1;
    private const int EggNeeded    = 1;

    /// <summary>
    /// When played, attempts to produce plain macarons up to productionCount,
    /// consuming ingredients as available.
    /// </summary>
    public override void Play(TurnManager turnManager,
                              ResourceManager resourceManager,
                              HandManager handManager,
                              ShopManager shopManager)
    {
        var macaronManager = Object.FindFirstObjectByType<MacaronManager>();
        if (macaronManager == null)
        {
            Debug.LogWarning("MacaronManager not found in scene. Cannot produce macarons.");
            return;
        }

        int produced = macaronManager.ProducePlain(productionCount, AlmondNeeded, SugarNeeded, EggNeeded);
    }
}

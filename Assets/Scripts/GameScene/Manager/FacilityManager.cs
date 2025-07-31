using UnityEngine;

public class FacilityManager : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private MacaronManager macaronManager;
    [SerializeField] private TurnManager turnManager;

    // Base production values per turn
    private int almondProduction = 2;
    private int sugarProduction  = 2;
    private int eggProduction    = 2;
    private int macaronProduction = 0;

    private void Awake()
    {
        // Bind production at end of each turn
        turnManager.OnTurnEnded.AddListener(ProduceAll);
    }

    /// <summary>
    /// Increase per-turn production for a given facility type.
    /// </summary>
    public void InstallFacility(FacilityType type, int amount)
    {
        switch (type)
        {
            case FacilityType.AlmondSupply:
                almondProduction += amount;
                break;
            case FacilityType.SugarSupply:
                sugarProduction += amount;
                break;
            case FacilityType.EggSupply:
                eggProduction += amount;
                break;
            case FacilityType.MacaronOven:
                macaronProduction += amount;
                break;
        }
    }

    /// <summary>
    /// Called at the end of each turn to apply all facility effects.
    /// </summary>
    private void ProduceAll()
    {
        // Supply basic ingredients
        resourceManager.AddResource("Almond", almondProduction);
        resourceManager.AddResource("Sugar",  sugarProduction);
        resourceManager.AddResource("Egg",    eggProduction);

        // Produce macarons directly via facility
        if (macaronProduction > 0 && macaronManager != null)
        {
            macaronManager.ProduceFacilityMacarons(macaronProduction);
        }
    }
}
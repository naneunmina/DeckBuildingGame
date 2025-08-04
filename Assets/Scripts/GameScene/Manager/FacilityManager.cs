using UnityEngine;
using UnityEngine.Events;

public class FacilityManager : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private MacaronManager macaronManager;
    [SerializeField] private TurnManager turnManager;

    // Base production values per turn
    private int almondProduction = 2;
    private int sugarProduction = 2;
    private int eggProduction = 2;
    private int macaronProduction = 0;

    public UnityEvent<int> OnAlmondProductionChanged;
    public UnityEvent<int> OnSugarProductionChanged;
    public UnityEvent<int> OnEggProductionChanged;
    public UnityEvent<int> OnMacaronProductionChanged;

    private void Awake()
    {
        // init events if null
        if (OnAlmondProductionChanged == null) OnAlmondProductionChanged = new UnityEvent<int>();
        if (OnSugarProductionChanged == null) OnSugarProductionChanged = new UnityEvent<int>();
        if (OnEggProductionChanged == null) OnEggProductionChanged = new UnityEvent<int>();
        if (OnMacaronProductionChanged == null) OnMacaronProductionChanged = new UnityEvent<int>();

        // bind production each turn
        turnManager.OnTurnChanged.AddListener(ProduceAll);

        // fire initial values so UI can pick them up
        OnAlmondProductionChanged.Invoke(almondProduction);
        OnSugarProductionChanged.Invoke(sugarProduction);
        OnEggProductionChanged.Invoke(eggProduction);
        OnMacaronProductionChanged.Invoke(macaronProduction);
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
                OnAlmondProductionChanged?.Invoke(almondProduction);
                break;
            case FacilityType.SugarSupply:
                sugarProduction += amount;
                OnSugarProductionChanged?.Invoke(sugarProduction);
                break;
            case FacilityType.EggSupply:
                eggProduction += amount;
                OnEggProductionChanged?.Invoke(eggProduction);
                break;
            case FacilityType.MacaronOven:
                macaronProduction += amount;
                OnMacaronProductionChanged?.Invoke(macaronProduction);
                break;
        }
    }

    /// <summary>
    /// Called at the end of each turn to apply all facility effects.
    /// </summary>
    private void ProduceAll(int num)
    {
        // Produce macarons directly via facility
        if (macaronProduction > 0 && macaronManager != null)
        {
            macaronManager.ProducePlain(macaronProduction, 1, 1, 1);
        }
        if (turnManager.currentTurn <= 1) return;
        // Supply basic ingredients
        resourceManager.AddResource(almondProduction, sugarProduction, eggProduction);

        
    }
    
    public int AlmondProduction  => almondProduction;
    public int SugarProduction   => sugarProduction;
    public int EggProduction     => eggProduction;
    public int MacaronProduction => macaronProduction;
}
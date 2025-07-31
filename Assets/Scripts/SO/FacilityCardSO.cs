using UnityEngine;

public enum FacilityType
{
    AlmondSupply,
    SugarSupply,
    EggSupply,
    MacaronOven
}

[CreateAssetMenu(fileName = "New Facility Card", menuName = "Deckaroon/FacilityCard")]
public class FacilityCardSO : CardSO
{
    [Header("Facility Settings")]
    [Tooltip("Type of facility to install")]
    public FacilityType facilityType;

    [Tooltip("Additional units produced per turn by this facility")]
    public int productionPerTurn = 1;

    /// <summary>
    /// When played, install or upgrade a facility via FacilityManager.
    /// </summary>
    public override void Play(TurnManager turnManager,
                              ResourceManager resourceManager,
                              HandManager handManager,
                              ShopManager shopManager)
    {
        var fm = Object.FindFirstObjectByType<FacilityManager>();
        if (fm != null)
        {
            fm.InstallFacility(facilityType, productionPerTurn);
        }
        else
        {
            Debug.LogWarning("FacilityManager not found in scene. Cannot install facility.");
        }
    }
}

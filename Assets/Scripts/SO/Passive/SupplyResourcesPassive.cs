using UnityEngine;

[CreateAssetMenu(menuName = "Deckaroon/Passives/SupplyFiveResources")]

public class SupplyResourcesPassive : PassiveSO
{
    public override void OnApply(PassiveManager mgr)
    {
        mgr.resourceManager.AddResource(5, 5, 5);
    }
}
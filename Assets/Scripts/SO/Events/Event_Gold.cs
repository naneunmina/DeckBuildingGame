using UnityEngine;

[CreateAssetMenu(fileName = "Event_Gold", menuName = "Deckaroon/Events/Gold")]
public class Event_Gold : EventSO
{
  [SerializeField] private int addGold;
  [SerializeField, Range(-1f, 1f)] private float addSilver;
  public override EventResult OnChoose1(TurnManager turnManager,
                            ResourceManager resourceManager,
                            ShopManager shopManager,
                            MacaronManager macaronManager,
                            FacilityManager facilityManager)
  {
    //금 선택
    turnManager.AddGold(addGold);
    if (addGold < 0) SfxEntry.I.PlayKey("Event_Bad");
    else if (addGold > 0) SfxEntry.I.PlayKey("Event_Good");
    return new EventResult { art = choice1Art, text = choice1Result };
  }

    public override EventResult OnChoose2(TurnManager turnManager,
                            ResourceManager resourceManager,
                            ShopManager shopManager,
                            MacaronManager macaronManager,
                            FacilityManager facilityManager)
    {
    //은 선택
    shopManager.SetSilverDropChance(addSilver);
    if (addSilver < 0) SfxEntry.I.PlayKey("Event_Bad");
    else if (addSilver > 0) SfxEntry.I.PlayKey("Event_Good");
      return new EventResult { art = choice2Art, text = choice2Result };
    }
}

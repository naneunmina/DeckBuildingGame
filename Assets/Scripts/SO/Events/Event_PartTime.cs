using UnityEngine;

[CreateAssetMenu(fileName = "Event_PartTime", menuName = "Deckaroon/Events/PartTime")]
public class Event_PartTime : EventSO
{
  public override EventResult OnChoose1(TurnManager turnManager,
                            ResourceManager resourceManager,
                            ShopManager shopManager,
                            MacaronManager macaronManager,
                            FacilityManager facilityManager)
  {
    //전단지알바
    //3골드
    //플레인 가격+1
    turnManager.AddGold(-3);
    macaronManager.PlusPlainPrice(1);
    SfxEntry.I.PlayKey("Event_Good");
    return new EventResult { art = choice1Art, text = choice1Result };
  }

    public override EventResult OnChoose2(TurnManager turnManager,
                            ResourceManager resourceManager,
                            ShopManager shopManager,
                            MacaronManager macaronManager,
                            FacilityManager facilityManager)
    {
    //인형탈알바
    //10골드
    //플레인+3
    turnManager.AddGold(-10);
    macaronManager.PlusPlainPrice(3);
    SfxEntry.I.PlayKey("Event_Good");
      return new EventResult { art = choice2Art, text = choice2Result };
    }
}

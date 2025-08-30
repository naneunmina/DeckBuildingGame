using UnityEngine;

[CreateAssetMenu(fileName = "Event_Cat", menuName = "Deckaroon/Events/Cat")]
public class Event_Cat : EventSO
{
  public override EventResult OnChoose1(TurnManager turnManager,
                            ResourceManager resourceManager,
                            ShopManager shopManager,
                            MacaronManager macaronManager,
                            FacilityManager facilityManager)
  {
    //고양이에게 마카롱을 나누어준다
    //마카롱 3개 제거
    //햄스터를 더 잡아옴 -> 턴당 아몬드 생산량 +5
    macaronManager.MinusPlain(3);
    facilityManager.InstallFacility(FacilityType.AlmondSupply, 5);
    return new EventResult { art = choice1Art, text = choice1Result };
  }

    public override EventResult OnChoose2(TurnManager turnManager,
                            ResourceManager resourceManager,
                            ShopManager shopManager,
                            MacaronManager macaronManager,
                            FacilityManager facilityManager)
    {
    //고양이를 내쫓는다
    //고양이 친구들을 더 데리고 와서 마카롱을 몽땅 빼앗겼다
    macaronManager.MinusPlain(100000);
      return new EventResult { art = choice2Art, text = choice2Result };
    }
}

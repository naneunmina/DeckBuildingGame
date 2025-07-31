using System;
using UnityEngine;

public class MacaronManager : MonoBehaviour
{
    [SerializeField] private ResourceManager resourceMgr;
    [SerializeField] private ScoreManager scoreMgr;

    private int plainCount;       // 이번 턴 생산된 일반 마카롱 수
    private int specialCount;     // 이번 턴 생산된 특수 마카롱 수
    /// <summary>
    /// 재료 소비 후 일반 마카롱 생산
    /// </summary>
    public int ProducePlain(int almondNeeded, int sugarNeeded, int eggNeeded)
    {
        // 가능한 생산 개수 계산
        int possible = Mathf.Min(resourceMgr.almond / almondNeeded,
                                resourceMgr.sugar / sugarNeeded,
                                resourceMgr.egg / eggNeeded);
        // 자원 차감
        resourceMgr.ConsumeResource("Almond", possible * almondNeeded);
        resourceMgr.ConsumeResource("Sugar", possible * sugarNeeded);
        resourceMgr.ConsumeResource("Egg", possible * eggNeeded);
        plainCount += possible;
        return possible;
    }

    /// <summary>
    /// 특수 마카롱 생산 (토핑 적용)
    /// </summary>
    public int ProduceSpecial(int toppingCardValue)
    {
        // 특수 마카롱은 생산 카드/토핑 카드로 추가 생산량 제공
        specialCount += toppingCardValue;
        return toppingCardValue;
    }

    /// <summary>
    /// 턴 종료 시 자동 판매 및 점수 계산 호출
    /// </summary>
    public void SellAllAndReset()
    {
        if (plainCount > 0)
            scoreMgr.AddPlain(plainCount);
        if (specialCount > 0)
            scoreMgr.AddSpecial(specialCount);
        plainCount = 0;
        specialCount = 0;
    }
    
    public void ProduceFacilityMacarons(int count)
    {
        plainCount += count;
    }
}

using System;
using UnityEngine;
using UnityEngine.Events;

public enum SpecialType { Special1,  Special3,  Special5 }

public class MacaronManager : MonoBehaviour
{
    [SerializeField] private ResourceManager resourceMgr;
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private ScoreManager scoreMgr;

    private int plainCount;       // 이번 턴 생산된 일반 마카롱 수
    private int[] specialCounts = new int[5];     // 이번 턴 생산된 특수 마카롱 수

    private int totalPlainCount = 0;
    private int[] totalSpecialCounts = new int[5];

    [SerializeField] private int plainPrice = 5;
    [SerializeField] private int[] specialPrices = { 10, 12, 15, 20, 25 };

    public UnityEvent<int> OnPlainCountChanged;
    public UnityEvent<SpecialType, int> OnSpecialCountChanged;
    /// <summary>
    /// 재료 소비 후 일반 마카롱 생산
    /// </summary>

    private void Awake()
    {
        if (OnPlainCountChanged == null) OnPlainCountChanged = new UnityEvent<int>();
        if (OnSpecialCountChanged == null) OnSpecialCountChanged = new UnityEvent<SpecialType, int>();

        turnManager.OnTurnEnded.AddListener(SellAllAndReset);

        // 초기 카운트(0) 알림
        OnPlainCountChanged.Invoke(plainCount);
        foreach (SpecialType type in System.Enum.GetValues(typeof(SpecialType)))
        {
            OnSpecialCountChanged.Invoke(type, specialCounts[(int)type]);
        }
    }
    public int ProducePlain(int desiredCount, int almondNeeded, int sugarNeeded, int eggNeeded)
    {
        int possible = Mathf.Min(
            desiredCount,
            resourceMgr.almond / almondNeeded,
            resourceMgr.sugar / sugarNeeded,
            resourceMgr.egg / eggNeeded
        );
        if (possible <= 0) return 0;

        resourceMgr.AddResource(-possible * almondNeeded, -possible * sugarNeeded, -possible * eggNeeded);

        plainCount += possible;
        totalPlainCount += possible;
        OnPlainCountChanged.Invoke(plainCount);
        return possible;
    }

    /// <summary>
    /// 특수 마카롱 생산 (토핑 적용)
    /// </summary>
    public void ProduceSpecial(int count, SpecialType type)
    {
        if (plainCount < count)
        {
            specialCounts[(int)type] += plainCount;
            totalSpecialCounts[(int)type] += plainCount;
            plainCount = 0;
        }
        else
        {
            plainCount -= count;
            specialCounts[(int)type] += count;
            totalSpecialCounts[(int)type] += count;
        }
        OnPlainCountChanged.Invoke(plainCount);
        OnSpecialCountChanged.Invoke(type, specialCounts[(int)type]);
    }

    /// <summary>
    /// 턴 종료 시 자동 판매 및 점수 계산 호출
    /// </summary>
    public void SellAllAndReset()
    {
        int plainRevenue = plainCount * plainPrice;
        if (plainRevenue > 0)
        {
            turnManager.AddGold(plainRevenue);
            scoreMgr.AddPlain(plainCount);
        }

        // Sell special macarons
        foreach (SpecialType type in System.Enum.GetValues(typeof(SpecialType)))
        {
            int idx = (int)type;
            int count = specialCounts[idx];
            if (count > 0)
            {
                int specialRevenue = count * specialPrices[idx];
                turnManager.AddGold(specialRevenue);
                scoreMgr.AddSpecial(count, idx);
            }
            specialCounts[idx] = 0;
            OnSpecialCountChanged.Invoke(type, 0);
        }

        // Reset plain count
        plainCount = 0;
        OnPlainCountChanged.Invoke(plainCount);
    }

    public int GetPlainCount() => plainCount;

    public int GetTotalPlainCount() => totalPlainCount;

    public int GetSpecialCount(SpecialType type)
    {
        return specialCounts[(int)type];
    }

    public int GetTotalSpecialCount(SpecialType type)
    {
        return totalSpecialCounts[(int)type];
    }

    public void MinusPlain(int amount)
    {
        if (plainCount < amount) plainCount = 0;
        else plainCount -= amount;
    }

    public void PlusPlainPrice(int amount)
    {
        plainPrice += amount;
    }
}

using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class TurnManager : MonoBehaviour
{
    [Header("Turn Settings")]
    public int maxTurns = 14;
    public float turnTimeSeconds = 90f;      // 1분 30초
    public int goldPerTurn = 10;

    [Header("Runtime State")]
    public int currentTurn { get; private set; }
    public float remainingTime { get; private set; }
    public int currentGold { get; private set; }

    // 이벤트: UI 등에 연결
    public UnityEvent<int> OnTurnChanged;
    public UnityEvent<float> OnTimerUpdated;
    public UnityEvent<int> OnGoldChanged;
    public UnityEvent OnTurnEnded;

    private Coroutine turnTimerCoroutine;

    void Start()
    {
        StartTurn(1);
    }

    public void StartTurn(int turn)
    {
        currentTurn = turn;
        remainingTime = turnTimeSeconds;
        AddGold(goldPerTurn);               // 매 턴 시작 시 골드 지급
        OnTurnChanged?.Invoke(currentTurn);

        // 타이머 시작
        if (turnTimerCoroutine != null) StopCoroutine(turnTimerCoroutine);
        turnTimerCoroutine = StartCoroutine(TurnTimer());
    }

    private IEnumerator TurnTimer()
    {
        while (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            OnTimerUpdated?.Invoke(remainingTime);
            yield return null;
        }
        EndTurn();
    }

    public void EndTurn()
    {
        if (turnTimerCoroutine != null) StopCoroutine(turnTimerCoroutine);

        // TODO: 재료·생산 시스템 호출 (ResourceManager, MacaronManager 등)
        OnTurnEnded?.Invoke();

        if (currentTurn < maxTurns)
            StartTurn(currentTurn + 1);
        else
            GameOver();
    }

    public void AddGold(int amount)
    {
        currentGold += amount;
        if (currentGold < 0) currentGold = 0;
        OnGoldChanged?.Invoke(currentGold);
    }

    public bool SpendGold(int amount)
    {
        if (currentGold < amount) return false;
        currentGold -= amount;
        OnGoldChanged?.Invoke(currentGold);
        return true;
    }

    private void GameOver()
    {
        ScoreboardUI.i.Open();
    }
}

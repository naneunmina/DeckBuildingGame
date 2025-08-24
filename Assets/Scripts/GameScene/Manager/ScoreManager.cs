using UnityEngine;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private float basePlainScore = 100f;
    [SerializeField] private float[] specialMultiplier = { 1.2f, 1.5f, 1.8f, 2.0f, 2.5f };

    public UnityEvent<int> OnScoreChanged;
    private float totalScore;
    private float plainBonusRate;    // 패시브/강화 합계
    private float specialBonusRate;
    private float questBonusRate;

    public void AddPlain(int count)
    {
        float score = count * basePlainScore * (1 + plainBonusRate) * (1 + questBonusRate);
        totalScore += score;
        OnScoreChanged?.Invoke(Mathf.FloorToInt(totalScore));
    }

    public void AddSpecial(int count, int level = 0)
    {
        float multi = specialMultiplier[level];
        float score = count * basePlainScore * multi * (1 + specialBonusRate) * (1 + questBonusRate);
        totalScore += score;
        OnScoreChanged?.Invoke(Mathf.FloorToInt(totalScore));
    }

    public int GetFinalScore()
    {
        return Mathf.FloorToInt(totalScore);
    }

    public bool CheckEndCondition(int currentTurn, int maxTurn)
    {
        return currentTurn >= maxTurn;
    }

    public float GetBasePlainScore() => basePlainScore;

    public void SetBasePlainScore(float newScore)
    {
        basePlainScore = newScore;
    }
}

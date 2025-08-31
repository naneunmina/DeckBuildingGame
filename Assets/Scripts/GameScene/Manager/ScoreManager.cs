using UnityEngine;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private float basePlainScore = 100f;
    [SerializeField] private float[] specialMultiplier = { 1.2f, 1.8f, 2.5f };

    public UnityEvent<int> OnScoreChanged;
    private float totalScore;

    public void AddPlain(int count)
    {
        float score = count * basePlainScore;
        totalScore += score;
        OnScoreChanged?.Invoke(Mathf.FloorToInt(totalScore));
    }

    public void AddSpecial(int count, int level = 0)
    {
        float multi = specialMultiplier[level];
        float score = count * basePlainScore * multi;
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

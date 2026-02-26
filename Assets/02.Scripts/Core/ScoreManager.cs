using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    private int _score;
    public int Score => _score;

    public static event Action<int> OnScoreChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        _score = 0;
    }

    public void AddScore(int amount)
    {
        _score += amount;
        OnScoreChanged?.Invoke(_score);
    }
}

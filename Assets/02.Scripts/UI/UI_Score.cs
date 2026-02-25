using TMPro;
using UnityEngine;

public class UI_Score : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;

    private void Start()
    {
        Refresh(ScoreManager.Instance.Score);
        ScoreManager.OnScoreChanged += Refresh;
    }

    private void OnDestroy()
    {
        ScoreManager.OnScoreChanged -= Refresh;
    }

    private void Refresh(int score)
    {
        _scoreText.text = $"Score: {score}";
    }
}

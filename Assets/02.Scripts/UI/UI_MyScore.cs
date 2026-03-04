using TMPro;
using UnityEngine;

public class UI_MyScore : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;

    private void Start()
    {
        Refresh();
        ScoreManager.OnScoreChanged += Refresh;
    }

    private void OnDestroy()
    {
        ScoreManager.OnScoreChanged -= Refresh;
    }

    private void Refresh()
    {
        _scoreText.text = $"Score : {ScoreManager.Instance.Score}";
    }
}

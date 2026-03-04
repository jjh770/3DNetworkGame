using TMPro;
using UnityEngine;

public class UI_ScoreItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _rankText;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _scoreText;

    public void Set(int rank, string nickname, int score)
    {
        _rankText.text = rank.ToString();
        _nameText.text = nickname;
        _scoreText.text = score.ToString();
    }
}

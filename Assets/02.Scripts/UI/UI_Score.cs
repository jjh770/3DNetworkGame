using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UI_Score : MonoBehaviourPunCallbacks
{
    [SerializeField] private UI_ScoreItem _scoreItemPrefab;
    [SerializeField] private Transform _container;
    private List<UI_ScoreItem> _scoreItems = new();

    public override void OnJoinedRoom()
    {
        // 기존 항목 정리 (이전 씬에서 남아있는 Item을 또 호출할 수 있으므로 정리해준다.)
        //  OnJoinedRoom이 같은 UI_Score 인스턴스에서 두 번 호출될 경우:
        // 첫 번째 OnJoinedRoom → _scoreItems에 4개 추가 → [item0, item1, item2, item3]
        // 두 번째 OnJoinedRoom → 또 4개 추가           → [item0, item1, item2, item3, item4, item5, item6, item7]
        // 리스트가 누적되기 때문에 이를 방지하는 방어 코드입니다.
        foreach (var item in _scoreItems)
        {
            if (item != null)
            {
                Destroy(item.gameObject);
            }
        }
        _scoreItems.Clear();

        int maxPlayers = PhotonNetwork.CurrentRoom.MaxPlayers;

        for (int i = 0; i < maxPlayers; i++)
        {
            var scoreItem = Instantiate(_scoreItemPrefab, _container);
            scoreItem.gameObject.SetActive(false);
            _scoreItems.Add(scoreItem);
        }

        ScoreManager.OnScoreChanged += Refresh;
        Refresh();
    }

    private void OnDestroy()
    {
        ScoreManager.OnScoreChanged -= Refresh;
    }

    private void Refresh()
    {
        var scores = ScoreManager.Instance.Scores;

        // 리드온리가 아니면 원본을 수정하므로 무결성 문제가 생긴다.
        List<ScoreData> scoreDatas = scores.Values.OrderByDescending(s => s.Score).ToList();

        for (int i = 0; i < _scoreItems.Count; i++)
        {
            bool isActive = i < scoreDatas.Count;
            _scoreItems[i].gameObject.SetActive(isActive);

            // todo: 3명 있는지 적절하게 반복문
            if (isActive)
            {
                _scoreItems[i].Set(i + 1, scoreDatas[i].Nickname, scoreDatas[i].Score);
            }
        }
    }
}

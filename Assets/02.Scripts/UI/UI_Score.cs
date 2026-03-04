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
    private void Refresh()
    {
        var scores = ScoreManager.Instance.Scores;

        // 리드온리가 아니면 원본을 수정하므로 무결성 문제가 생긴다.
        List<ScoreData> scoreDatas = scores.Values.OrderByDescending(s => s.Score).ToList();

        // 1. todo: 1등부터 3등까지 정렬     
        //          - 정렬은 이미 매니저에서 해서 넘겨야 하나 vs
        //          - UI에서 해야 하나...   (도메인 규칙에 따라 다르다.)
        //          - 정리 과제: Linq 사용 
        //          -             ㄴ 무엇인지, 언제쓰이는지, 장단점은 무엇인지

        // 2. 점수 1,000점마다 플레이어 무기의 scale이 0.1씩 증가한다. (동기화)
        // 3. 죽으면 점수의 반이 사라진다.
        // 플레이어 수가 바뀌면 아이템 재생성

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

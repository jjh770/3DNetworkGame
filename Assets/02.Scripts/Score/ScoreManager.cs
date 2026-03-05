using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class ScoreManager : MonoBehaviourPunCallbacks
{
    public static ScoreManager Instance { get; private set; }

    private int _score;
    public int Score => _score;

    private Dictionary<int, ScoreData> _scores = new();
    // 외부 수정이 불가능하도록 ReadOnlyDictionary로 감싸서 제공
    public ReadOnlyDictionary<int, ScoreData> Scores => new ReadOnlyDictionary<int, ScoreData>(_scores);

    public static event Action OnScoreChanged;

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
    }

    public void AddScore(int amount)
    {
        _score += amount;
        Refresh();
    }

    private void Start()
    {
        if (!PhotonNetwork.InRoom) return;

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.ContainsKey("score"))
            {
                _scores[player.ActorNumber] = new ScoreData()
                {
                    Nickname = player.NickName,
                    Score = (int)player.CustomProperties["score"]
                };
            }
        }

        // 네트워크 응답 기다리지 않고 즉시 로컬 플레이어 추가
        _scores[PhotonNetwork.LocalPlayer.ActorNumber] = new ScoreData()
        {
            Nickname = PhotonNetwork.NickName,
            Score = _score
        };

        Refresh();           // Photon에 점수 업로드
        OnScoreChanged?.Invoke(); // UI 즉시 갱신
    }

    //public override void OnJoinedRoom()
    //{
    //    // 기존 플레이어들의 점수 정보를 먼저 가져오고 내꺼를 Refresh()
    //    foreach (Player player in PhotonNetwork.PlayerList)
    //    {
    //        if (player.CustomProperties.ContainsKey("score"))
    //        {
    //            _scores[player.ActorNumber] = new ScoreData()
    //            {
    //                Nickname = player.NickName,
    //                Score = (int)player.CustomProperties["score"]
    //            };
    //        }
    //    }
    //    Refresh(); // 내 점수를 Photon에 올림 -> 다른 플레이어들이 OnPlayerPropertiesUpdate에서 내 점수 정보를 받아서 _scores에 저장한다.
    //    OnScoreChanged?.Invoke();
    //}

    private void Refresh()
    {
        // 해시 테이블은 딕셔너리와 같은 키-값 형태로 저장하는데, 키 값의 자료형은 object다.
        Hashtable hashtable = new Hashtable();
        hashtable.Add("score", _score);

        // 프로퍼티 설정 - 로컬 플레이어의 커스텀 프로퍼티에 점수 정보를 저장한다. (Photon 전용)
        PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!changedProps.ContainsKey("score")) return;

        ScoreData scoreData = new ScoreData()
        {
            Nickname = targetPlayer.NickName,
            Score = (int)changedProps["score"]
        };

        _scores[targetPlayer.ActorNumber] = scoreData;
        OnScoreChanged?.Invoke();
    }
}

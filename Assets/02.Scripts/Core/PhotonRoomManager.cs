using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine;

public class PhotonRoomManager : MonoBehaviourPunCallbacks
{
    public static PhotonRoomManager Instance { get; private set; }

    private Room _room;
    public Room Room => _room;

    public event Action OnRoomInfoChanged; // 룸 정보가 바뀌었을 때
    public event Action<Player> OnPlayerEnter; // 플레이어가 들어왔을 때
    public event Action<Player> OnPlayerLeft; // 플레이어가 나갔을 때
    public event Action<string, string> OnPlayerDeathed; // 플레이어를 처치하였을 때
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public override void OnJoinedRoom()
    {
        _room = PhotonNetwork.CurrentRoom;

        OnRoomInfoChanged?.Invoke();

        // 리소스 폴더에서 "Player" 프리팹을 찾아 생성(인스턴스화)하도록 한다. + 서버에 등록도 함.
        // -> 리소스 폴더는 잘 쓰이지 않음. 다른 방법으로 해결하기
        Transform spawnPoint = SpawnManager.Instance.SpawnPlayer();
        PhotonNetwork.Instantiate("Player", spawnPoint.position, spawnPoint.rotation);
    }

    // 새로운 플레이어가 방에 입장하면 자동으로 호출되는 함수
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        OnRoomInfoChanged?.Invoke();
        OnPlayerEnter?.Invoke(newPlayer);
    }

    // 플레이어가 방에서 퇴장하면 자동으로 호출되는 함수
    public override void OnPlayerLeftRoom(Player leftPlayer)
    {
        OnRoomInfoChanged?.Invoke();
        OnPlayerLeft?.Invoke(leftPlayer);
    }

    public void OnPlayerDeath(int attackerActorNumber, int victimActorNumber)
    {
        string attackerNickname = Room.Players[attackerActorNumber].NickName;
        string victimNickname = Room.Players[victimActorNumber].NickName;

        OnPlayerDeathed?.Invoke(attackerNickname, victimNickname);
    }
}

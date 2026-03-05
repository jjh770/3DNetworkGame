using Photon.Pun;
using Photon.Realtime;
using System;

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

    public override void OnCreatedRoom()
    {
        PhotonNetwork.LoadLevel("GameScene"); // 방장
    }

    public override void OnJoinedRoom()
    {
        _room = PhotonNetwork.CurrentRoom;
        OnRoomInfoChanged?.Invoke();

        if (!PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("GameScene"); // 비마스터도 명시적으로 로드
        }
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

    public void OnPlayerDeath(int attackerActorNumber, AttackerType attackerType, int victimActorNumber)
    {
        string attackerNickname = attackerType == AttackerType.Player ?
            Room.Players[attackerActorNumber].NickName :
            ParseMonsterName(PhotonView.Find(attackerActorNumber).gameObject.name);

        string victimNickname = Room.Players[victimActorNumber].NickName;

        OnPlayerDeathed?.Invoke(attackerNickname, victimNickname);
    }

    private string ParseMonsterName(string monsterObjectName)
    {
        return monsterObjectName.Split('(')[0]; // "Bear(Clone)" -> "Bear"
    }

    // 방장이 바뀌었을 떄 커스텀 방 프로퍼티를 업데이트 해줌 (방장 닉네임)
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        OnRoomInfoChanged?.Invoke();

        if (!newMasterClient.IsLocal) return;

        ExitGames.Client.Photon.Hashtable newRoomInfos = new ExitGames.Client.Photon.Hashtable
        {
            { "MasterNickname", PhotonNetwork.NickName }
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(newRoomInfos);
    }
}

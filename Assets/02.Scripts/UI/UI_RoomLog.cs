using Photon.Realtime;
using TMPro;
using UnityEngine;

public class UI_RoomLog : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _logText;

    private void Start()
    {
        _logText.text = "방에 입장했습니다.";
        PhotonRoomManager.Instance.OnPlayerEnter += PlayerEnterLog;
        PhotonRoomManager.Instance.OnPlayerLeft += PlayerLeftLog;
        PhotonRoomManager.Instance.OnPlayerDeathed += PlayerDeathLog;
    }
    private void OnDestroy()
    {
        PhotonRoomManager.Instance.OnPlayerEnter -= PlayerEnterLog;
        PhotonRoomManager.Instance.OnPlayerLeft -= PlayerLeftLog;
        PhotonRoomManager.Instance.OnPlayerDeathed -= PlayerDeathLog;
    }
    private void PlayerEnterLog(Player newPlayer)
    {
        _logText.text += "\n" + $"{newPlayer.NickName}님이 입장하였습니다.";
    }

    private void PlayerLeftLog(Player leftPlayer)
    {
        _logText.text += "\n" + $"{leftPlayer.NickName}님이 퇴장하였습니다.";
    }

    private void PlayerDeathLog(string attackerNickname, string victimNickname)
    {
        _logText.text += "\n" + $"{attackerNickname}님이 {victimNickname}을 처치했습니다!";
    }
}

using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_RoomItem : MonoBehaviourPunCallbacks
{
    [SerializeField] private TextMeshProUGUI _roomNameTextUI;
    [SerializeField] private TextMeshProUGUI _masterNicknameTextUI;
    [SerializeField] private TextMeshProUGUI _roomCapacityTextUI;
    [SerializeField] private Button _roomEnterButton;

    private RoomInfo _roomInfo;

    private void Start()
    {
        _roomEnterButton.onClick.AddListener(EnterRoom);
    }

    public void Init(RoomInfo roomInfo)
    {
        _roomInfo = roomInfo;

        _roomNameTextUI.text = roomInfo.Name;

        var nickName = roomInfo.CustomProperties.TryGetValue("MasterNickname", out var name) ? name.ToString() : "알 수 없음";
        _masterNicknameTextUI.text = nickName;
        _roomCapacityTextUI.text = $"{roomInfo.PlayerCount} / {roomInfo.MaxPlayers}";
    }

    private void EnterRoom()
    {
        if (_roomInfo == null)
        {
            Debug.LogError("방 정보가 없습니다.");
            return;
        }
        PhotonNetwork.JoinRoom(_roomInfo.Name);
    }
}

using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_RoomInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _roomNameText;
    [SerializeField] private TextMeshProUGUI _roomCapacityText;
    [SerializeField] private Button _exitButton;

    private void Start()
    {
        _exitButton.onClick.AddListener(ExitRoom);

        PhotonRoomManager.Instance.OnRoomInfoChanged += Refresh;

        Refresh();
    }

    private void Refresh()
    {
        Room room = PhotonRoomManager.Instance.Room;
        if (room == null) return;

        _roomNameText.text = room.Name;
        _roomCapacityText.text = $"{room.PlayerCount} / {room.MaxPlayers}";
    }

    private void ExitRoom()
    {
        // todo 구현예정
    }
}

using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;

public class UI_RoomList : MonoBehaviourPunCallbacks
{
    private List<UI_RoomItem> _roomItems;
    private Dictionary<string, RoomInfo> _rooms = new Dictionary<string, RoomInfo>();

    private void Awake()
    {
        _roomItems = GetComponentsInChildren<UI_RoomItem>().ToList();

        HideAllRoomUI();
    }

    private void HideAllRoomUI()
    {
        foreach (UI_RoomItem roomItem in _roomItems)
        {
            roomItem.gameObject.SetActive(false);
        }
    }

    // 로비에 입장 후 방 목록(내용, 개수, 이름 등)이 변경되었을 떄 자동으로 호출되는 함수
    // 처음에는 모든 방 리스트를 제공하지만 그 이후는 바뀐 방 정보만 제공함
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        HideAllRoomUI();

        foreach (var room in roomList)
        {
            // 삭제된 방이 있을 경우
            if (room.RemovedFromList)
            {
                _rooms.Remove(room.Name); // 딕셔너리에서 제거한다.
                continue;
            }
            _rooms[room.Name] = room; // 딕셔너리에 자동으로 추가되거나 업데이트 된다.
        }

        int roomCount = _rooms.Count;
        List<RoomInfo> rooms = _rooms.Values.ToList();
        for (int i = 0; i < roomCount; i++)
        {
            // 방 개수만큼 UI를 활성화한다.
            _roomItems[i].Init(rooms[i]);
            _roomItems[i].gameObject.SetActive(true);
        }

    }
}

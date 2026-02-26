using Photon.Pun;
using UnityEngine;

public class ItemObjectFactory : MonoBehaviour
{
    public static ItemObjectFactory Instance { get; private set; }
    private PhotonView _photonView;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        _photonView = GetComponent<PhotonView>();
    }

    // 우리의 약속 : 방장에게 룸 관련해서 뭔가 요청을 할 때는 메서드 명에 Request로 시작하는 게 유지보수가 편하다.
    public void RequestDropItems(Vector3 dropPosition)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // 방장이라면 그냥 호출
            DropItems(dropPosition);
        }
        else
        {
            // 방장이 아니라면 방장의 함수를 호출
            _photonView.RPC(nameof(DropItems), RpcTarget.MasterClient, dropPosition);
        }
    }

    [PunRPC]
    public void DropItems(Vector3 dropPosition)
    {
        int randomCount = UnityEngine.Random.Range(3, 5);
        for (int i = 0; i < randomCount; i++)
        {
            // 플레이어가 룸을 나가면 그 플레이어가 생성/소유한 모든 네트워크 게임 오브젝트는 사라진다.
            // 플레이어 생명 주기를 가지고 있다. -> 룸 생명 주기로 변경해야함.
            // PhotonNetwork.Instantiate("ScoreItem", dropPosition, Quaternion.identity, 0,
            PhotonNetwork.InstantiateRoomObject("ScoreItem", dropPosition, Quaternion.identity, 0,
                new object[] { PhotonNetwork.LocalPlayer.ActorNumber });

            // 포톤에는 룸 안에 방장(Master Client)이 있다.
            // 방을 만든 사람이 방장
            // 방장을 양도할 수 있음.
            // 방장이 게임을 나가면 자동으로 그 다음으로 들어온 사람이 방장이 됨.
            // InstantiateRoomObject는 방장만이 가능함.
            // 방장이 아닌 플레이어는 방장에게 요청해야함.
        }
    }
}

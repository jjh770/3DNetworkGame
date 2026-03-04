using Photon.Pun;
using UnityEngine;

// IPunInstasntiageMaginCallback
// PhotonNetwork.Instantiate로 생성된 오브젝트가 네트워크 초기화를 완료한 직후 호출되는 콜백
public class ScoreItem : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    [SerializeField] private int _scoreAmount = 10;
    private int _dropperActorNumber = -1;

    // 생성과 동시에 PhotonView 세팅, 소유권 등록 등 네트워크 초기화가 필요합니다.
    // 이 초기화가 끝난 시점을 알려주는 게 이 콜백 OnPhotonInstantiate.
    // InstantiationData[0]에는 아이템을 드랍한 플레이어의 ActorNumber가 들어있다.
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        _dropperActorNumber = (int)info.photonView.InstantiationData[0];
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out PlayerController player)) return;
        if (!player.PhotonView.IsMine) return;
        if (player.PhotonView.Owner.ActorNumber == _dropperActorNumber) return;

        ScoreManager.Instance.AddScore(_scoreAmount);
        ItemObjectFactory.Instance.RequestDestroyItem(photonView.ViewID);
    }
}

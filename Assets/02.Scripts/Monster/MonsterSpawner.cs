using Photon.Pun;
using UnityEngine;

public class MonsterSpawner : MonoBehaviourPunCallbacks
{
    [SerializeField] private Transform[] _spawnPoints;

    public override void OnJoinedRoom()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        foreach (Transform point in _spawnPoints)
        {
            PhotonNetwork.InstantiateRoomObject("BearMonster", point.position, point.rotation);
        }
    }
}

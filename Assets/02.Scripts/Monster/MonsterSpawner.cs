using Photon.Pun;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] _spawnPoints;

    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        foreach (Transform point in _spawnPoints)
        {
            PhotonNetwork.InstantiateRoomObject("BearMonster", point.position, point.rotation);
        }
    }
}

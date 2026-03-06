using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private float _spawnDelay = 5f;

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

    public Transform SpawnPlayer()
    {
        Transform spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];
        return spawnPoint;
    }

    public void RequestRespawn(PlayerController player)
    {
        Debug.Log($"{player.PhotonView.name}플레이어 리스폰 요청 받음");
        StartCoroutine(RespawnPlayer(player));
    }

    private IEnumerator RespawnPlayer(PlayerController player)
    {
        yield return new WaitForSeconds(_spawnDelay);
        if (player == null) yield return null; // 리스폰 코루틴이 진행되는 동안 플레이어가 나갔을 수도 있으므로 null 체크
        Transform spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];
        player.GetAbility<PlayerRespawnAbility>().Respawn(spawnPoint);
    }
}

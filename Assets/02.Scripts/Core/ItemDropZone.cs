using Photon.Pun;
using UnityEngine;

public class ItemDropZone : MonoBehaviour
{
    [SerializeField] private float _radius = 10f;
    [SerializeField] private float _spawnHeight = 40f;
    [SerializeField] private float _spawnInterval = 1f;

    private float _timer;

    private void Update()
    {
        // 방장만 타이머 돌림 → SpawnItem도 방장만 호출하므로 Request 불필요
        if (!PhotonNetwork.IsMasterClient) return;

        _timer += Time.deltaTime;
        if (_timer < _spawnInterval) return;

        _timer = 0f;

        Vector2 randomCircle = Random.insideUnitCircle * _radius;
        Vector3 spawnPos = new Vector3(
            transform.position.x + randomCircle.x,
            _spawnHeight,
            transform.position.z + randomCircle.y
        );
        ItemObjectFactory.Instance.SpawnSkyDropItem(spawnPos);
    }
}

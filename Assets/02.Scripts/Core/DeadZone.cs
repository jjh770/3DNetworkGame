using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out PlayerController player)) return;
        if (!player.PhotonView.IsMine) return;

        SpawnManager.Instance.RequestRespawn(player);
    }
}

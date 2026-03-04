using Photon.Pun;
using UnityEngine;

public class SkyDropItem : MonoBehaviourPun
{
    [SerializeField] private int _scoreAmount = 20;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out PlayerController player)) return;
        if (!player.PhotonView.IsMine) return;

        ScoreManager.Instance.AddScore(_scoreAmount);
        ItemObjectFactory.Instance.RequestDestroyItem(photonView.ViewID);
    }
}

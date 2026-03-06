using Photon.Pun;
using UnityEngine;

public class BearWeaponCollider : MonoBehaviour
{
    private BearCombat _combat;
    private Collider _collider;

    private void Awake()
    {
        _combat = GetComponentInParent<BearCombat>();
        _collider = GetComponent<Collider>();
        _collider.enabled = false;
    }

    public void EnableCollider() => _collider.enabled = true;
    public void DisableCollider() => _collider.enabled = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        PlayerHitAbility hitAbility = other.GetComponentInParent<PlayerHitAbility>();
        if (hitAbility == null) return;

        hitAbility.GetComponentInParent<PhotonView>().RPC(nameof(PlayerHitAbility.TakeDamage), RpcTarget.All,
            _combat.Stat.AttackDamage, _combat.GetComponent<PhotonView>().ViewID, AttackerType.Monster);
    }
}

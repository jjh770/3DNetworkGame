using Photon.Pun;
using UnityEngine;

public class PlayerWeaponHitAbility : PlayerAbility
{
    protected override void OnUpdate()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (!_owner.PhotonView.IsMine) return;

        if (other.transform == _owner.transform) return;

        if (other.TryGetComponent(out IDamageable damageable))
        {
            // 상대방의 TakeDamage를 RPC로 호출해서, 상대방의 방에서도 데미지가 적용되도록 한다.
            PlayerController player = other.GetComponent<PlayerController>();
            player.PhotonView.RPC(nameof(PlayerHitAbility.TakeDamage), RpcTarget.All, _owner.Stat.AttackDamage);

            _owner.GetAbility<PlayerWeaponColliderAbility>().DeactiveCollider();
        }
    }
}

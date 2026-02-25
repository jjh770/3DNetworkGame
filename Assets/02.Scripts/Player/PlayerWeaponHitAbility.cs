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
            // 포톤에서는 Room 안에서 플레이어마다 고유 식별자(ID)인 ActorNumber를 가지고 있다.
            // 나 자신의 포톤 ActorNumber LocalPlayer == _owner.PhotonView.Owner
            //int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
            int actorNumber = _owner.PhotonView.Owner.ActorNumber;

            // 상대방의 TakeDamage를 RPC로 호출해서, 상대방의 방에서도 데미지가 적용되도록 한다.
            PlayerController player = other.GetComponent<PlayerController>();
            player.PhotonView.RPC(nameof(PlayerHitAbility.TakeDamage), RpcTarget.All, _owner.Stat.AttackDamage, actorNumber);

            _owner.GetAbility<PlayerWeaponColliderAbility>().DeactiveCollider();
        }
    }
}

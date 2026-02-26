using Photon.Pun;
using System;
using UnityEngine;
public class PlayerHitAbility : PlayerAbility, IDamageable
{
    [SerializeField] private Animator _animator;
    public static event Action<float, float> OnHitPlayer;
    protected override void OnUpdate()
    {

    }

    [PunRPC] // TakeDamage는 내 방에서의 상대 플레이어가 아닌 상대 방의 상대 플레이어에게 줘야하므로 RPC로 호출한다.
    public void TakeDamage(float damage, int attackerActorNumber)
    {
        if (_owner.Stat.IsDead) return;

        // 모든 클라이언트에서 체력 감소
        _owner.Stat.TakeDamage(damage);

        // 내 클라이언트만 카메라 흔들리기
        if (_owner.PhotonView.IsMine)
        {
            OnHitPlayer?.Invoke(3, 1);
        }

        // 죽었을 때 사망판정 및 UI로그 업데이트, 리스폰
        if (_owner.Stat.IsDead)
        {
            Debug.Log($"{_owner.PhotonView.name}이(가) 사망했습니다.");
            PlayDieAnimation();
            if (_owner.PhotonView.IsMine)
            {
                // 아이템 드롭
                ItemObjectFactory.Instance.RequestDropItems(transform.position);
                //_owner.GetAbility<PlayerItemDropAbility>()?.DropItems(transform.position);
            }
            PhotonRoomManager.Instance.OnPlayerDeath(attackerActorNumber);
            _owner.PhotonView.RPC("PlayDieAnimation", RpcTarget.Others);
            SpawnManager.Instance.RequestRespawn(_owner);
        }
    }

    [PunRPC]
    private void PlayDieAnimation()
    {
        _animator.SetTrigger("Die");
    }
}

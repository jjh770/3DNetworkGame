using System;
using UnityEngine;

[Serializable]
public class BearStat
{
    [Header("순찰 관련 스탯")]
    public float PatrolWaitTime;

    [Header("이동 관련 스탯")]
    public float Speed;
    public float ChaseSpeed;
    public float AngularSpeed;
    public float Acceleration;

    [Header("공격 관련 스탯")]
    public float AttackWaitTime;
    public float AttackDelayTime;
    public float AttackDamage;

    [Header("체력 관련 스탯")]
    public float MaxHealth;
    public float CurrentHealth;

    [Header("피격 관련 스탯")]
    public float KnockbackForce;
    public float KnockbackTime;

    public void Initialize()
    {
        CurrentHealth = MaxHealth;
    }

    // 체력 관리
    public void TakeDamage(float amount)
    {
        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
    }

    // 상태 체크
    public bool IsDead => CurrentHealth <= 0;
}

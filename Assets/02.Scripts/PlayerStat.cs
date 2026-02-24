using System;
using UnityEngine;

[Serializable]
public class PlayerStat
{
    [Header("이동 관련 스탯")]
    public float MoveSpeed;
    public float RunSpeed;
    public float RunStamina;
    public float RotationSpeed;

    [Header("공격 관련 스탯")]
    public float AttackCoolTime;
    public float AttackStamina;

    [Header("점프 관련 스탯")]
    public float JumpPower;
    public float JumpStamina;

    [Header("체력 관련 스탯")]
    public float MaxHealth;
    public float CurrentHealth;

    [Header("스태미나 관련 스탯")]
    public float MaxStamina;
    public float CurrentStamina;
    public float GainStamina;

    public void Initialize()
    {
        CurrentHealth = MaxHealth;
        CurrentStamina = MaxStamina;
    }

    // 체력 관리
    public void TakeDamage(float amount)
    {
        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
    }

    public void RecoverHealth(float amount)
    {
        CurrentHealth = Mathf.Min(MaxHealth, CurrentHealth + amount);
    }

    // 스태미나 관리
    public bool TryConsumeStamina(float amount)
    {
        if (CurrentStamina >= amount)
        {
            CurrentStamina -= amount;
            return true;  // 소비 성공
        }
        return false;  // 스태미나 부족
    }

    public void RecoverStamina(float amount)
    {
        CurrentStamina = Mathf.Min(MaxStamina, CurrentStamina + amount);
    }

    // 상태 체크
    public bool IsDead => CurrentHealth <= 0;
    public bool HasStamina => CurrentStamina > 0;
}

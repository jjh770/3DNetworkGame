using UnityEngine;

public class PlayerAttackAbility : PlayerAbility
{
    private float _attackTimer = 0f;

    [SerializeField] private Animator _animator;
    [SerializeField] private float _comboResetTime = 1.0f;
    private int _comboStep = 0;
    private float _lastAttackTime = 0f;

    protected override void OnUpdate()
    {
        if (_animator == null) return;

        _attackTimer += Time.deltaTime;

        if (Time.time - _lastAttackTime > _comboResetTime)
        {
            _comboStep = 0;
        }

        if (Input.GetMouseButtonDown(0) && _attackTimer >= _owner.Stat.AttackCoolTime)
        {
            if (!_owner.Stat.TryConsumeStamina(_owner.Stat.AttackStamina))
            {
                Debug.Log("스태미나 부족 : 공격");
                return;
            }

            _attackTimer = 0f;

            _animator.SetTrigger($"Attack{Random.Range(1, 4)}");
        }

        if (Input.GetMouseButtonDown(1) && _attackTimer >= _owner.Stat.AttackCoolTime)
        {
            if (!_owner.Stat.TryConsumeStamina(_owner.Stat.AttackStamina))
            {
                Debug.Log("스태미나 부족 : 공격");
                return;
            }

            _attackTimer = 0f;

            _lastAttackTime = Time.time;
            _comboStep++;

            if (_comboStep > 3)
            {
                _comboStep = 1;
            }

            _animator.SetTrigger($"Attack{_comboStep}");
        }
    }
}

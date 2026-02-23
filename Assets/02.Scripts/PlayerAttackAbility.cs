using UnityEngine;

public class PlayerAttackAbility : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private float _comboResetTime = 1.0f;
    private int _comboStep = 0;
    private float _lastAttackTime = 0f;

    private void Update()
    {
        if (_animator == null) return;

        if (Time.time - _lastAttackTime > _comboResetTime)
        {
            _comboStep = 0;
        }

        if (Input.GetMouseButtonDown(0))
        {
            _animator.SetTrigger($"Attack{Random.Range(1, 4)}");
        }

        if (Input.GetMouseButtonDown(1))
        {
            _lastAttackTime = Time.time;
            _comboStep++;

            if (_comboStep > 3)
                _comboStep = 1;

            _animator.SetTrigger($"Attack{_comboStep}");
        }
    }
}

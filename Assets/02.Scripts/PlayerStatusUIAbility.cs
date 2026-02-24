using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUIAbility : PlayerAbility
{
    [SerializeField] private Image _healthGauge;
    [SerializeField] private Image _staminaGauge;
    protected override void OnUpdate()
    {
        if (_healthGauge != null && _owner.Stat.MaxHealth > 0)
        {
            _healthGauge.fillAmount = Mathf.Clamp01(_owner.Stat.CurrentHealth / _owner.Stat.MaxHealth);
        }

        if (_staminaGauge != null && _owner.Stat.MaxStamina > 0)
        {
            _staminaGauge.fillAmount = Mathf.Clamp01(_owner.Stat.CurrentStamina / _owner.Stat.MaxStamina);
        }
    }
}

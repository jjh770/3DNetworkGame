using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUIAbility : PlayerAbility
{
    [SerializeField] private Image _healthGauge;
    [SerializeField] private Image _staminaGauge;

    private void Start()
    {
        _owner.OnStatSynced += UpdateUI;
    }
    private void OnDestroy()
    {
        _owner.OnStatSynced -= UpdateUI;
    }

    private void UpdateUI()
    {
        if (_healthGauge != null && _owner.Stat.MaxHealth > 0)
            _healthGauge.fillAmount = Mathf.Clamp01(_owner.Stat.CurrentHealth / _owner.Stat.MaxHealth);

        if (_staminaGauge != null && _owner.Stat.MaxStamina > 0)
            _staminaGauge.fillAmount = Mathf.Clamp01(_owner.Stat.CurrentStamina / _owner.Stat.MaxStamina);
    }

    // 로컬 플레이어 UI는 매 프레임 갱신 (OnPhotonSerializeView 없이도 동작)
    protected override void OnUpdate()
    {
        UpdateUI();
    }
}

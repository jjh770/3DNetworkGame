using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUI : MonoBehaviour
{
    [SerializeField] private Image _healthGauge;
    [SerializeField] private Image _staminaGauge;

    private PlayerController _owner;

    private void Awake()
    {
        _owner = GetComponentInParent<PlayerController>();
    }

    private void Start()
    {
        _owner.OnStatSynced += UpdateUI;
    }
    private void OnDestroy()
    {
        _owner.OnStatSynced -= UpdateUI;
    }
    private void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (_healthGauge != null && _owner.Stat.MaxHealth > 0)
            _healthGauge.fillAmount = Mathf.Clamp01(_owner.Stat.CurrentHealth / _owner.Stat.MaxHealth);

        if (_staminaGauge != null && _owner.Stat.MaxStamina > 0)
            _staminaGauge.fillAmount = Mathf.Clamp01(_owner.Stat.CurrentStamina / _owner.Stat.MaxStamina);
    }
}

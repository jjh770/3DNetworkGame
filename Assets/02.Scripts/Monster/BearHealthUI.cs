using UnityEngine;
using UnityEngine.UI;

public class BearHealthUI : MonoBehaviour
{
    [SerializeField] private Image _healthGauge;
    private BearController _owner;

    void Start()
    {
        _owner = GetComponentInParent<BearController>();
        _owner.OnHealthChanged += UpdateUI;
    }

    private void OnDestroy()
    {
        _owner.OnHealthChanged -= UpdateUI;
    }

    private void UpdateUI()
    {
        if (_healthGauge != null && _owner.Stat.MaxHealth > 0)
            _healthGauge.fillAmount = Mathf.Clamp01(_owner.Stat.CurrentHealth / _owner.Stat.MaxHealth);
    }
}

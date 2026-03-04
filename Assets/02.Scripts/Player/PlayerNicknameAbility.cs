using TMPro;
using UnityEngine;

public class PlayerNicknameAbility : PlayerAbility
{
    [SerializeField] private TextMeshProUGUI _nicknameTextUI;

    private void Start()
    {
        _nicknameTextUI.text = _owner.NickName;
        if (_owner.IsMine)
        {
            _nicknameTextUI.color = Color.green;
        }
        else
        {
            _nicknameTextUI.color = Color.red;
        }
    }

    protected override void OnUpdate()
    {

    }
}

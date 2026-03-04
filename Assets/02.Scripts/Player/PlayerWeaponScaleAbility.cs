using UnityEngine;

public class PlayerWeaponScaleAbility : MonoBehaviour
{
    private PlayerController _controller;
    private Vector3 _originalScale;

    private void Awake()
    {
        _controller = GetComponentInParent<PlayerController>();
        _originalScale = transform.localScale;
    }

    private void Start()
    {
        ScoreManager.OnScoreChanged += Refresh;
    }

    private void OnDestroy()
    {
        ScoreManager.OnScoreChanged -= Refresh;
    }

    private void Refresh()
    {
        int actorNumber = _controller.PhotonView.Owner.ActorNumber;
        if (!ScoreManager.Instance.Scores.TryGetValue(actorNumber, out ScoreData data))
        {
            return;
        }

        float scaleMultiflier = 1 + (data.Score / 100f) * 0.1f;
        transform.localScale = _originalScale * scaleMultiflier;
    }
}

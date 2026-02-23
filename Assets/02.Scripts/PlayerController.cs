using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PhotonView PhotonView;
    private void Awake()
    {
        PhotonView = GetComponent<PhotonView>();
    }
}

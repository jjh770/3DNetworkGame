using UnityEngine;

public class PhotonRoomManager : MonoBehaviour
{
    public static PhotonRoomManager Instance { get; set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

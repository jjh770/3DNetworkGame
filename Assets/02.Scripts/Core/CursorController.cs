using UnityEngine;

public class CursorController : MonoBehaviour
{
    private void Update()
    {
        Cursor.lockState = Input.GetKey(KeyCode.LeftAlt) ? CursorLockMode.Locked : CursorLockMode.None;
    }
}

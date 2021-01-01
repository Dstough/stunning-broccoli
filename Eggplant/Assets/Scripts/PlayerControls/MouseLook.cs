using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public Transform playerCamera;
    public float mouseSensitivity = 100f;

    private float x = 0f;
    private float y = 0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        x += -Input.GetAxis("Mouse Y") * mouseSensitivity;
        y += Input.GetAxis("Mouse X") * mouseSensitivity;

        x = Mathf.Clamp(x, -90, 90);

        //TODO: Smooth this out somehow.
        playerCamera.localRotation = Quaternion.Euler(x, 0, 0);
        transform.localRotation = Quaternion.Euler(0, y, 0);
    }
}

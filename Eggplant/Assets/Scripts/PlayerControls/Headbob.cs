using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class Headbob : MonoBehaviour
{
    public Transform playerCamera;
    public float headBobSpeed = 6f;
    public float headBobAmount = 1f;
    public float cameraResetSpeed = 20f;

    private Vector3 cameraRestPosition;
    private float timer = Mathf.PI / 2;

    void Start()
    {
        cameraRestPosition = playerCamera.position;
    }

    void Update()
    {
        var grounded = GetComponent<PlayerMovement>().grounded;

        if (grounded && (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0))
        {
            timer += headBobSpeed * Time.deltaTime;
            playerCamera.transform.localPosition = new Vector3()
            {
                x = Mathf.Cos(timer) * headBobAmount,
                y = cameraRestPosition.y + Mathf.Abs(Mathf.Sin(timer) * headBobAmount),
                z = cameraRestPosition.z
            };
        }
        else
        {
            timer = Mathf.PI / 2;
            playerCamera.transform.localPosition = new Vector3()
            {
                x = Mathf.Lerp(playerCamera.transform.localPosition.x, cameraRestPosition.x, cameraResetSpeed * Time.deltaTime),
                y = Mathf.Lerp(playerCamera.transform.localPosition.y, cameraRestPosition.y, cameraResetSpeed * Time.deltaTime),
                z = Mathf.Lerp(playerCamera.transform.localPosition.z, cameraRestPosition.z, cameraResetSpeed * Time.deltaTime)
            };
        }

        if (timer > Mathf.PI * 2)
            timer = 0;
    }
}

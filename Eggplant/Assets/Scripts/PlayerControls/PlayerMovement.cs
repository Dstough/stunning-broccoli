using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private const float gravity = -9.81f;
    
    [Space(10)]
    [Header("Reference Hookups")]
    public CharacterController controller;
    public Transform groundCheck;
    public LayerMask groundMask;
    public Transform playerCamera;
    [Space(10)]
    [Header("Movement & Sensitivity")]
    public float groundDistance = .1f;
    public float mouseSensitivity = 100f;
    public float movementSpeed = 12f;
    public float jumpHeight = 3f;
    private Vector3 velocity;
    private Vector3 MoveDirection;
    private Vector3 JumpDirection;
    private float xRotation = 0f;
    private float strafeLeanRotation = 0f;
    private bool grounded = true;
    [Space(10)]
    [Header("Camera Effects")]
    public float strafeLeanAmmount = 0.2f;
    public float cameraResetSpeed = 20f;
    public float headBobSpeed = 7f;
    public float headBobAmount = 0.1f;
    private Vector3 cameraRestPosition;
    private float timer = Mathf.PI / 2;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cameraRestPosition = playerCamera.transform.localPosition;
    }

    void Update()
    {
        Move();
        Look();
        HeadBob();
        HeadLean();
    }

    private void HeadBob()
    {
        if (grounded && (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0))
        {
            timer += headBobSpeed * Time.deltaTime;
            playerCamera.transform.localPosition = new Vector3()
            {
                x = Mathf.Cos(timer) * headBobAmount,
                y = cameraRestPosition.y + Mathf.Abs(Mathf.Sin(timer * 2) * headBobAmount),
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

    private void HeadLean()
    {
        var x = Input.GetAxis("Horizontal");

        if (Math.Abs(strafeLeanRotation) < strafeLeanAmmount && x < 0 && grounded)
            strafeLeanRotation += Time.deltaTime * strafeLeanAmmount * 2;
        if (Math.Abs(strafeLeanRotation) < strafeLeanAmmount && x > 0 && grounded)
            strafeLeanRotation -= Time.deltaTime * strafeLeanAmmount * 2;

        if (strafeLeanRotation > 0 && x >= 0)
            strafeLeanRotation -= Time.deltaTime * strafeLeanAmmount * 4;
        if (strafeLeanRotation < 0 && x <= 0)
            strafeLeanRotation += Time.deltaTime * strafeLeanAmmount * 4;
    }

    private void Look()
    {
        var mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        var mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, strafeLeanRotation);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void Move()
    {
        grounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (grounded && velocity.y < 0)
            velocity.y = -2f;

        var x = Input.GetAxis("Horizontal");
        var z = Input.GetAxis("Vertical");

        MoveDirection = Vector3.ClampMagnitude(transform.right * x + transform.forward * z, 1) * movementSpeed * Time.deltaTime;

        if (grounded)
        {
            controller.Move(new Vector3(MoveDirection.x, 0, MoveDirection.z));
            JumpDirection = MoveDirection;
        }
        else
            controller.Move(new Vector3(JumpDirection.x, 0, JumpDirection.z));

        if (Input.GetButtonDown("Jump") && grounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
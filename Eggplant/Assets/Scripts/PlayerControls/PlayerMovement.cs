using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Transform playerCamera;
    public Transform groundCheck;
    public LayerMask groundMask;
    public CharacterController controller;

    public float mouseSensitivity = 100f;
    public float movementSpeed = 12f;
    public float groundDistance = .1f;
    public float jumpHeight = 3f;
    public float strafeLeanAmmount = 0.2f;
    public float walkingBobbingSpeed = 14f;
    public float bobbingAmount = 0.5f;

    private float timer = 0f;
    private float xRotation = 0f;
    private float strafeLeanRotation = 0f;
    private float headBobRotation = 0f;
    private bool grounded = true;
    private const float gravity = -9.81f;
    private Vector3 velocity;
    private Vector3 MoveDirection;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Headbob();
        HeadLean();
        Look();
        Move();
    }

    private void Headbob()
    {
        var z = Input.GetAxis("Vertical");

        if (z < -0.001 || z > 0.001 || headBobRotation < -0.05f || headBobRotation > 0.05f)
        {
            timer += Time.deltaTime;
            headBobRotation += bobbingAmount * Mathf.Cos(timer * walkingBobbingSpeed) ;
        }
        else
        {
            timer = 0f;
        }
    }

    private void HeadLean()
    {
        var x = Input.GetAxis("Horizontal");

        if (Math.Abs(strafeLeanRotation) < strafeLeanAmmount && x < 0)
            strafeLeanRotation += Time.deltaTime * strafeLeanAmmount * 2;
        if (Math.Abs(strafeLeanRotation) < strafeLeanAmmount && x > 0)
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
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, strafeLeanRotation + headBobRotation);
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
        controller.Move(new Vector3(MoveDirection.x, 0, MoveDirection.z));

        if (Input.GetButtonDown("Jump") && grounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
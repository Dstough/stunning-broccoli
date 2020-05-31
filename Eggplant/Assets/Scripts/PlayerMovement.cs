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

    private float xRotation = 0f;
    private bool grounded = true;
    private const float gravity = -9.81f;
    private Vector3 velocity;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Look();
        Move();
    }

    private void Look()
    {
        var mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        var mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void Move()
    {
        grounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (grounded && velocity.y < 0)
            velocity.y = -2f;

        var x = Input.GetAxis("Horizontal");
        var z = Input.GetAxis("Vertical");
        var move = transform.right * x + transform.forward * z;
        var targetMove = move * movementSpeed * Time.deltaTime;

        controller.Move(new Vector3(targetMove.x, 0, targetMove.z));

        if (Input.GetButtonDown("Jump") && grounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
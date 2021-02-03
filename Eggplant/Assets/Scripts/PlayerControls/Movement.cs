using UnityEngine;

namespace Assets.Scripts.PlayerControls
{
    [RequireComponent(typeof(CharacterController))]
    public class Movement : MonoBehaviour
    {
        public float movementSpeed = 12f;
        public float gravity = -9.81f;
        public float jumpHeight = 3f;
        public bool grounded = false;

        private CharacterController controller;
        private float x = 0f;
        private float z = 0f;
        private Vector3 gravityVelocity;
        private LayerMask groundMask;
        private float groundDistance = 0.4f;

        private void Start()
        {
            controller = GetComponent<CharacterController>();
            gravityVelocity = new Vector3(0, 0, 0);
            groundMask = LayerMask.GetMask(new string[] { "Ground" });
        }

        private void Update()
        {
            x = Input.GetAxis("Horizontal");
            z = Input.GetAxis("Vertical");
            grounded = Physics.CheckSphere(
                new Vector3(transform.position.x, transform.position.y - controller.height / 2f, transform.position.z),
                groundDistance,
                groundMask
            );

            if (grounded && gravityVelocity.y < 0)
                gravityVelocity.y = -2f;

            var move = transform.right * x + transform.forward * z;
            controller.Move(move * movementSpeed * Time.deltaTime);

            if (Input.GetButtonDown("Jump") && grounded)
                gravityVelocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);

            gravityVelocity.y += gravity * Time.deltaTime ;
            controller.Move(gravityVelocity * Time.deltaTime);
        }
    }
}

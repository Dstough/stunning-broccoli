using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerMovement : MonoBehaviour
{
    public float speed = 5.0f;
    public float gravity = 9.8f;
    public float maxVelocityChange = 10.0f;
    public bool canJump = true;
    public float jumpHeight = 2.0f;
    public bool grounded = false;

    private LayerMask groundMask;
    private CapsuleCollider capsuleCollider;
    private Rigidbody myRigidbody;

    void Start()
    {
        groundMask = LayerMask.GetMask(new string[] { "Ground" });
        capsuleCollider = GetComponent<CapsuleCollider>();
        myRigidbody = GetComponent<Rigidbody>();

        myRigidbody.freezeRotation = true;
        myRigidbody.useGravity = false;
    }

    void FixedUpdate()
    {
        if (grounded)
        {
            // Calculate how fast we should be moving
            var targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            targetVelocity = transform.TransformDirection(targetVelocity);
            targetVelocity *= speed;

            // Apply a force that attempts to reach our target velocity
            var velocity = myRigidbody.velocity;
            var velocityChange = targetVelocity - velocity;
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;
            myRigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
        
            // Jump
            if (canJump && Input.GetButton("Jump"))
            {
                myRigidbody.velocity = new Vector3(myRigidbody.velocity.x, CalculateJumpVerticalSpeed(), myRigidbody.velocity.z);
            }
        }

        // We apply gravity manually for more tuning control
        if (!grounded)
            myRigidbody.AddForce(new Vector3(0, -gravity * myRigidbody.mass, 0));

        if (IsOnSlope() && Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, capsuleCollider.height, groundMask)) 
               transform.position = new Vector3(hit.point.x, hit.point.y + capsuleCollider.height/2, hit.point.z);
        grounded = false;
    }

    void OnCollisionStay()
    {
        grounded = Physics.CheckSphere
        (
            new Vector3(transform.position.x, transform.position.y - capsuleCollider.height / 2f, transform.position.z),
            capsuleCollider.radius - .1f,
            groundMask
        );
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere
        (
            new Vector3(transform.position.x, transform.position.y - GetComponent<CapsuleCollider>().height / 2f, transform.position.z),
            GetComponent<CapsuleCollider>().radius - .1f
        );
    }

    bool IsOnSlope()
    {
        var value = false;
        
        if (Physics.CheckSphere(new Vector3(transform.position.x, transform.position.y - capsuleCollider.height / 2f, transform.position.z), capsuleCollider.radius - .1f, groundMask))
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, capsuleCollider.height, groundMask))
                value = hit.normal != Vector3.up;

        return value;
    }

    float CalculateJumpVerticalSpeed()
    {
        // From the jump height and gravity we deduce the upwards speed 
        // for the character to reach at the apex.
        return Mathf.Sqrt(2 * jumpHeight * gravity);
    }
}
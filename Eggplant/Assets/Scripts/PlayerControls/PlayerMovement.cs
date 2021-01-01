using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerMovement : MonoBehaviour
{
    public LayerMask groundMask;
    public bool grounded = true;
    public float movementSpeed = 6f;
    public float jumpForce = 6f;
    public float maxSlopeAngle = .5f;

    private Rigidbody rigidBody;
    private CapsuleCollider capsuleCollider;
    private Vector3 jumpPosition;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        jumpPosition = new Vector3();
    }

    void Update()
    {
        var x = Input.GetAxisRaw("Horizontal");
        var z = Input.GetAxisRaw("Vertical");
        var movementPosition = Vector3.ClampMagnitude(transform.right * x + transform.forward * z, 1) * movementSpeed;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, capsuleCollider.height / 2f + .1f, groundMask))
            grounded = hit.normal.y > maxSlopeAngle;
        else
            grounded = false;

        //TODO: Deal with the slope problem.

        if (grounded)
        {
            jumpPosition = movementPosition;

            if (Input.GetButtonDown("Jump"))
                rigidBody.velocity = new Vector3(rigidBody.velocity.x, jumpForce, rigidBody.velocity.z);
            else
                rigidBody.velocity = new Vector3(rigidBody.velocity.x, -jumpForce, rigidBody.velocity.z);
        }

        rigidBody.velocity = grounded
            ? new Vector3(movementPosition.x, rigidBody.velocity.y, movementPosition.z)
            : new Vector3(jumpPosition.x, rigidBody.velocity.y, jumpPosition.z);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - (GetComponent<CapsuleCollider>().height / 2f + .1f), transform.position.z));
    }
}
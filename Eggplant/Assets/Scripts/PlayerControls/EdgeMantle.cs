using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]

public class EdgeMantle : MonoBehaviour
{
    Vector3 baseRayPosition, baseRaycastForwardPosition, baseRaycastPositive45Position, baseRaycastNegative45Position;
    Vector3 stepRayPosition, stepRaycastForwardPosition, stepRaycastPositive45Position, stepRaycastNegative45Position;
    Vector3 jumpRayPosition, jumpRaycastForwardPosition;

    CapsuleCollider capsuleCollider;
    PlayerMovement playerMovement;
    Rigidbody myRigidbody;
    LayerMask groundMask;

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
        playerMovement = GetComponent<PlayerMovement>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        groundMask = LayerMask.GetMask(new string[] { "Ground" });
    }

    void Update()
    {
        baseRayPosition = new Vector3(transform.position.x, transform.position.y - capsuleCollider.height / 2 + .1f, transform.position.z);
        stepRayPosition = new Vector3(transform.position.x, transform.position.y - capsuleCollider.height / 4 + .1f, transform.position.z);
        jumpRayPosition = new Vector3(transform.position.x, transform.position.y + capsuleCollider.height / 2 - .1f, transform.position.z);

        if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
            return;

        //if (Physics.Raycast(baseRayPosition, transform.forward, .6f, groundMask))
        //    if (!Physics.Raycast(stepRayPosition, transform.forward, .6f, groundMask))
        //        ;
    }

    void OnDrawGizmos()
    {
        baseRaycastForwardPosition = baseRayPosition + transform.TransformDirection(Vector3.forward);
        baseRaycastPositive45Position = baseRayPosition + Vector3.ClampMagnitude(transform.TransformDirection(Vector3.forward + Vector3.right), 1);
        baseRaycastNegative45Position = baseRayPosition + Vector3.ClampMagnitude(transform.TransformDirection(Vector3.forward - Vector3.right), 1);

        stepRaycastForwardPosition = stepRayPosition + transform.TransformDirection(Vector3.forward);
        stepRaycastPositive45Position = stepRayPosition + Vector3.ClampMagnitude(transform.TransformDirection(Vector3.forward + Vector3.right), 1);
        stepRaycastNegative45Position = stepRayPosition + Vector3.ClampMagnitude(transform.TransformDirection(Vector3.forward - Vector3.right), 1);

        jumpRaycastForwardPosition = jumpRayPosition + transform.TransformDirection(Vector3.forward);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(baseRayPosition, baseRaycastForwardPosition);
        Gizmos.DrawLine(baseRayPosition, baseRaycastPositive45Position);
        Gizmos.DrawLine(baseRayPosition, baseRaycastNegative45Position);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(stepRayPosition, stepRaycastForwardPosition);
        Gizmos.DrawLine(stepRayPosition, stepRaycastPositive45Position);
        Gizmos.DrawLine(stepRayPosition, stepRaycastNegative45Position);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(jumpRayPosition, jumpRaycastForwardPosition);
    }
}

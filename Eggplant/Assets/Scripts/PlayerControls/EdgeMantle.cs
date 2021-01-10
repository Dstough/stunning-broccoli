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

    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    void Update()
    {
        baseRayPosition = new Vector3(transform.position.x, transform.position.y - capsuleCollider.height / 2 + .1f, transform.position.z);
        baseRaycastForwardPosition = baseRayPosition + transform.TransformDirection(Vector3.forward);
        baseRaycastPositive45Position = baseRayPosition + Vector3.ClampMagnitude(transform.TransformDirection(Vector3.forward + Vector3.right), 1);
        baseRaycastNegative45Position = baseRayPosition + Vector3.ClampMagnitude(transform.TransformDirection(Vector3.forward - Vector3.right), 1);

        stepRayPosition = new Vector3(transform.position.x, transform.position.y - capsuleCollider.height / 4 + .1f, transform.position.z);
        stepRaycastForwardPosition = stepRayPosition + transform.TransformDirection(Vector3.forward);
        stepRaycastPositive45Position = stepRayPosition + Vector3.ClampMagnitude(transform.TransformDirection(Vector3.forward + Vector3.right), 1);
        stepRaycastNegative45Position = stepRayPosition + Vector3.ClampMagnitude(transform.TransformDirection(Vector3.forward - Vector3.right), 1);

        jumpRayPosition = new Vector3(transform.position.x, transform.position.y + capsuleCollider.height / 2 - .1f, transform.position.z);
        jumpRaycastForwardPosition = jumpRayPosition + transform.TransformDirection(Vector3.forward);
    }

    void OnDrawGizmos()
    {
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

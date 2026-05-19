using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RagdollRootFollower : MonoBehaviour
{
    [Header("Target To Follow")]
    public Transform playerRoot;

    [Header("Position Follow")]
    public float positionStrength = 80f;
    public float positionDamping = 12f;

    [Header("Rotation Follow")]
    public float rotationStrength = 80f;
    public float rotationDamping = 10f;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    void FixedUpdate()
    {
        if (playerRoot == null) return;

        FollowPosition();
        FollowRotation();
    }

    void FollowPosition()
    {
        Vector3 toTarget = playerRoot.position - rb.position;

        Vector3 force = toTarget * positionStrength;
        force -= rb.linearVelocity * positionDamping;

        rb.AddForce(force, ForceMode.Acceleration);
    }

    void FollowRotation()
    {
        Quaternion targetRotation = playerRoot.rotation;
        Quaternion currentRotation = rb.rotation;

        Quaternion difference = targetRotation * Quaternion.Inverse(currentRotation);
        difference.ToAngleAxis(out float angle, out Vector3 axis);

        if (angle > 180f)
            angle -= 360f;

        if (Mathf.Abs(angle) < 0.01f) return;

        axis.Normalize();

        Vector3 torque = axis * angle * Mathf.Deg2Rad * rotationStrength;
        torque -= rb.angularVelocity * rotationDamping;

        rb.AddTorque(torque, ForceMode.Acceleration);
    }
}
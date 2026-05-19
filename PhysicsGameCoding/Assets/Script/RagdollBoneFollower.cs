using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RagdollBoneFollower : MonoBehaviour
{
    [Header("Target Animated Bone")]
    public Transform targetBone;

    [Header("Follow Settings")]
    public float followStrength = 300f;
    public float followDamping = 30f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    private void FixedUpdate()
    {
        if (targetBone == null || rb == null) return;

        Quaternion targetRotation = targetBone.rotation;
        Quaternion currentRotation = rb.rotation;

        Quaternion rotationDifference = targetRotation * Quaternion.Inverse(currentRotation);
        rotationDifference.ToAngleAxis(out float angle, out Vector3 axis);

        if (angle > 180f)
            angle -= 360f;

        if (Mathf.Abs(angle) < 0.01f) return;

        axis.Normalize();

        Vector3 torque = axis * angle * Mathf.Deg2Rad * followStrength;
        torque -= rb.angularVelocity * followDamping;

        rb.AddTorque(torque, ForceMode.Acceleration);
    }
}
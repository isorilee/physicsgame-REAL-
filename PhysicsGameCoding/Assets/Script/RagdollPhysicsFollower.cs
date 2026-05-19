using UnityEngine;

public class RagdollPhysicsFollower : MonoBehaviour
{
    public enum DrunkState
    {
        Sober,
        Tipsy,
        Drunk,
        Wasted
    }

    [System.Serializable]
    public class BonePair
    {
        [Header("Animated Bone")]
        public Transform animatedBone;

        [Header("Ragdoll Rigidbody")]
        public Rigidbody ragdollBody;
    }

    [Header("Bone Connections")]
    public BonePair[] bones;

    [Header("Root Follow")]
    public Transform animatedRoot;
    public Rigidbody ragdollRoot;

    [Header("Drunk State")]
    public DrunkState currentState = DrunkState.Sober;

    [Header("Spring Strength")]
    public float soberStrength = 900f;
    public float tipsyStrength = 600f;
    public float drunkStrength = 300f;
    public float wastedStrength = 120f;

    [Header("Physics")]
    public float damper = 50f;

    [Header("Root Movement")]
    public float rootFollowSpeed = 15f;
    public float rootRotateSpeed = 10f;

    private float currentStrength;

    private void Start()
    {
        SetupRigidbodies();
        ApplyDrunkState(currentState);
    }

    private void FixedUpdate()
    {
        if (currentState == DrunkState.Sober)
            return;

        FollowRoot();
        FollowBones();
    }

    void SetupRigidbodies()
    {
        foreach (BonePair bone in bones)
        {
            if (bone.ragdollBody == null)
                continue;

            bone.ragdollBody.interpolation = RigidbodyInterpolation.Interpolate;
            bone.ragdollBody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            bone.ragdollBody.maxAngularVelocity = 20f;
        }

        if (ragdollRoot != null)
        {
            ragdollRoot.interpolation = RigidbodyInterpolation.Interpolate;
            ragdollRoot.collisionDetectionMode = CollisionDetectionMode.Continuous;
            ragdollRoot.maxAngularVelocity = 20f;
        }
    }

    void FollowRoot()
    {
        if (animatedRoot == null || ragdollRoot == null)
            return;

        Vector3 targetPos = animatedRoot.position;
        Vector3 velocity = (targetPos - ragdollRoot.position) * rootFollowSpeed;

        ragdollRoot.linearVelocity = velocity;

        Quaternion targetRot = animatedRoot.rotation;
        Quaternion deltaRot = targetRot * Quaternion.Inverse(ragdollRoot.rotation);

        deltaRot.ToAngleAxis(out float angle, out Vector3 axis);

        if (angle > 180f)
            angle -= 360f;

        if (axis == Vector3.zero)
            return;

        Vector3 angularVel =
            axis.normalized *
            angle *
            Mathf.Deg2Rad *
            rootRotateSpeed;

        ragdollRoot.angularVelocity = angularVel;
    }

    void FollowBones()
    {
        foreach (BonePair bone in bones)
        {
            if (bone.animatedBone == null || bone.ragdollBody == null)
                continue;

            Quaternion targetRot = bone.animatedBone.rotation;
            Quaternion currentRot = bone.ragdollBody.rotation;

            Quaternion deltaRot = targetRot * Quaternion.Inverse(currentRot);
            deltaRot.ToAngleAxis(out float angle, out Vector3 axis);

            if (angle > 180f)
                angle -= 360f;

            if (axis == Vector3.zero)
                continue;

            Vector3 torque =
                axis.normalized *
                angle *
                Mathf.Deg2Rad *
                currentStrength;

            torque -= bone.ragdollBody.angularVelocity * damper;

            bone.ragdollBody.AddTorque(torque, ForceMode.Acceleration);
        }
    }

    public void ApplyDrunkState(DrunkState state)
    {
        currentState = state;

        switch (state)
        {
            case DrunkState.Sober:
                currentStrength = soberStrength;
                SetRagdollActive(false);
                break;

            case DrunkState.Tipsy:
                currentStrength = tipsyStrength;
                SnapRagdollToAnimation();
                SetRagdollActive(true);
                break;

            case DrunkState.Drunk:
                currentStrength = drunkStrength;
                SetRagdollActive(true);
                break;

            case DrunkState.Wasted:
                currentStrength = wastedStrength;
                SetRagdollActive(true);
                break;
        }
    }

    void SetRagdollActive(bool active)
    {
        if (ragdollRoot != null)
        {
            ragdollRoot.isKinematic = !active;
            ragdollRoot.useGravity = active;

            Collider rootCollider = ragdollRoot.GetComponent<Collider>();
            if (rootCollider != null)
                rootCollider.enabled = active;
        }

        foreach (BonePair bone in bones)
        {
            if (bone.ragdollBody == null)
                continue;

            bone.ragdollBody.isKinematic = !active;
            bone.ragdollBody.useGravity = active;

            Collider col = bone.ragdollBody.GetComponent<Collider>();
            if (col != null)
                col.enabled = active;
        }
    }

    void SnapRagdollToAnimation()
    {
        if (animatedRoot != null && ragdollRoot != null)
        {
            ragdollRoot.position = animatedRoot.position;
            ragdollRoot.rotation = animatedRoot.rotation;
            ragdollRoot.linearVelocity = Vector3.zero;
            ragdollRoot.angularVelocity = Vector3.zero;
        }

        foreach (BonePair bone in bones)
        {
            if (bone.animatedBone == null || bone.ragdollBody == null)
                continue;

            bone.ragdollBody.position = bone.animatedBone.position;
            bone.ragdollBody.rotation = bone.animatedBone.rotation;
            bone.ragdollBody.linearVelocity = Vector3.zero;
            bone.ragdollBody.angularVelocity = Vector3.zero;
        }
    }
}
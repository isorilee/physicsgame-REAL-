using UnityEngine;

public class RagdollPhysicsOnly : MonoBehaviour
{
    [Header("Hide Ragdoll Visuals")]
    public bool hideRagdollMesh = true;

    [Header("Physics Settings")]
    public bool disableAnimator = true;
    public bool keepPhysicsActive = true;

    private void Awake()
    {
        if (hideRagdollMesh)
            HideAllRenderers();

        if (disableAnimator)
            DisableAnimators();

        if (keepPhysicsActive)
            EnablePhysics();
    }

    private void HideAllRenderers()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);

        foreach (Renderer r in renderers)
        {
            r.enabled = false;
        }
    }

    private void DisableAnimators()
    {
        Animator[] animators = GetComponentsInChildren<Animator>(true);

        foreach (Animator animator in animators)
        {
            animator.enabled = false;
        }
    }

    private void EnablePhysics()
    {
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>(true);

        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        Collider[] colliders = GetComponentsInChildren<Collider>(true);

        foreach (Collider col in colliders)
        {
            col.enabled = true;
        }
    }
}
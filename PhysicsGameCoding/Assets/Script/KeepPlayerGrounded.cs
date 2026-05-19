using UnityEngine;

public class KeepPlayerGrounded : MonoBehaviour
{
    [Header("References")]
    public Rigidbody rb;
    public Transform groundCheck;

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.45f;
    public float groundCheckDistance = 0.7f;

    [Header("Anti Float")]
    public float downForce = 35f;
    public float maxUpwardVelocity = 2f;
    public float maxHeightFromGround = 1.2f;

    private bool isGrounded;

    void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (rb == null || groundCheck == null) return;

        CheckGround();
        StopFloating();
    }

    void CheckGround()
    {
        isGrounded = Physics.SphereCast(
            groundCheck.position,
            groundCheckRadius,
            Vector3.down,
            out RaycastHit hit,
            groundCheckDistance,
            groundLayer,
            QueryTriggerInteraction.Ignore
        );
    }

    void StopFloating()
    {
        // If player is grounded, keep them pressed down slightly
        if (isGrounded)
        {
            rb.AddForce(Vector3.down * downForce, ForceMode.Acceleration);
        }

        // Prevent random physics bumps from launching the player upward too much
        if (rb.linearVelocity.y > maxUpwardVelocity)
        {
            rb.linearVelocity = new Vector3(
                rb.linearVelocity.x,
                maxUpwardVelocity,
                rb.linearVelocity.z
            );
        }

        //if player is not grounded, pull down harder
        if (!isGrounded)
        {
            rb.AddForce(Vector3.down * downForce * 1.5f, ForceMode.Acceleration);
        }
    }
}
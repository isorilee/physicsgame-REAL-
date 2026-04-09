using UnityEngine;

public class SpinAttack : MonoBehaviour
{
    // how fast the player spins (this gets applied as torque, not just rotation)
    public float spinSpeed = 1000f;

    // how strong the push is on nearby objects
    public float force = 20f;

    // how far the spin effect reaches
    public float radius = 5f;

    // small forward movement so the player doesn’t just spin in place
    public float forwardPush = 5f;

    // adding a bit of randomness makes it feel less controlled (more chaotic / funny)
    public float instability = 200f;

    private Rigidbody rb;

    void Start()
    {
        // get the rigidbody once at start so we don’t keep calling GetComponent every frame
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // using FixedUpdate because this is all physics-based
        // (forces and torque should be applied here instead of Update)

        if (Input.GetKey(KeyCode.S))
        {
            // apply torque so the player actually spins using physics
            //  better than transform.Rotate because it interacts with collisions
            rb.AddTorque(Vector3.up * spinSpeed);

            // slight forward push so the player becomes a moving "hazard"
            // otherwise spinning in place isn’t very interesting
            rb.AddForce(transform.forward * forwardPush);

            // add some instability so it doesn’t feel perfectly controlled
            //  make the movement a bit unpredictable (which is the point)
            rb.AddTorque(Random.insideUnitSphere * instability);

            // find all colliders in a radius around the player
            // basically what objects are close enough to be affected
            Collider[] hits = Physics.OverlapSphere(transform.position, radius);

            foreach (Collider hit in hits)
            {
                // try to get a rigidbody from whatever we hit
                Rigidbody otherRb = hit.GetComponent<Rigidbody>();

                // ignore objects without rigidbodies (like static environment)
                // also ignore our own rigidbody so we don’t push ourselves
                if (otherRb != null && otherRb != rb)
                {
                    // direction from player → object
                    // this determines which way the object gets launched
                    Vector3 dir = (hit.transform.position - transform.position).normalized;

                    // scale force based on how fast we’re spinning
                    // so faster spin = more chaos
                    float spinMultiplier = rb.angularVelocity.magnitude;

                    // push the object outward
                    otherRb.AddForce(dir * force * spinMultiplier, ForceMode.Impulse);

                    // add a bit of upward force so things don’t just slide on the ground
                    // this helps create that "objects flying everywhere" effect
                    otherRb.AddForce(Vector3.up * force * 0.5f, ForceMode.Impulse);

                    // add random rotation to the object so it tumbles
                    // makes everything feel less stiff and more chaotic
                    otherRb.AddTorque(Random.insideUnitSphere * 10f, ForceMode.Impulse);
                }
            }
        }
    }
}
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectGrabber : MonoBehaviour
{
    [Header("Grab Settings")]
    [Tooltip("How far away the player can grab objects from")]
    public float grabRange = 4f;

    [Tooltip("How fast the held object moves to the hold point")]
    public float holdSmoothing = 15f;

    [Tooltip("The point in front of the camera where the object is held")]
    public Transform holdPoint;

    [Tooltip("How much force is applied when throwing")]
    public float throwForce = 15f;

    private Rigidbody heldObject;
    private bool isHolding = false;

    // Start is called once before the first frame update
    void Update()
    {
        // Optional:
        // If your FPSplayer script is already doing highlighting,
        // leave this OFF to avoid conflicts.
        // UpdateHighlight();
    }

    void FixedUpdate()
    {
        if (isHolding && heldObject != null)
        {
            MoveHeldObject();
        }
    }

    void TryGrab()
    {
        // Block grabbing before drinking
        if (DrunkManager.instance == null || !DrunkManager.instance.isDrunk)
        {
            Debug.Log("Too sober to grab anything.");
            return;
        }

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * grabRange, Color.yellow, 0.5f);

        if (Physics.Raycast(ray, out hit, grabRange))
        {
            // Do NOT grab interactable objects like the bottle
            InteractableObject interactable = hit.collider.GetComponentInParent<InteractableObject>();
            if (interactable != null)
            {
                Debug.Log("This is interactable, not grabbable.");
                return;
            }

            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                heldObject = rb;

                PhysicsObjects physObj = heldObject.GetComponent<PhysicsObjects>();
                if (physObj != null)
                {
                    physObj.isHeld = true;
                }

                heldObject.useGravity = false;
                heldObject.freezeRotation = true;
                heldObject.linearVelocity = Vector3.zero;
                heldObject.angularVelocity = Vector3.zero;

                isHolding = true;

                Debug.Log("Grabbed: " + heldObject.name);
            }
        }
    }

    void MoveHeldObject()
    {
        if (heldObject == null || holdPoint == null) return;

        Vector3 targetPos = holdPoint.position;
        Vector3 currentPos = heldObject.position;

        Vector3 newPos = Vector3.Lerp(currentPos, targetPos, holdSmoothing * Time.fixedDeltaTime);
        heldObject.MovePosition(newPos);
    }

    void DropObject()
    {
        if (heldObject == null) return;

        PhysicsObjects physObj = heldObject.GetComponent<PhysicsObjects>();
        if (physObj != null)
        {
            physObj.isHeld = false;
        }

        heldObject.useGravity = true;
        heldObject.freezeRotation = false;
        heldObject.linearVelocity = Vector3.zero;
        heldObject.angularVelocity = Vector3.zero;

        heldObject = null;
        isHolding = false;
    }

    void ThrowObject()
    {
        if (heldObject == null) return;

        PhysicsObjects physObj = heldObject.GetComponent<PhysicsObjects>();
        if (physObj != null)
        {
            physObj.isHeld = false;
        }

        heldObject.useGravity = true;
        heldObject.freezeRotation = false;
        heldObject.AddForce(Camera.main.transform.forward * throwForce, ForceMode.Impulse);

        heldObject = null;
        isHolding = false;
    }

    public void OnGrabPerformed(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (isHolding)
        {
            DropObject();
        }
        else
        {
            TryGrab();
        }
    }

    public void OnThrowPerformed(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (isHolding)
        {
            ThrowObject();
        }
    }

    // Only use this if FPSplayer is NOT already handling highlight
    void UpdateHighlight()
    {
        if (isHolding) return;

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * grabRange, Color.red);

        if (Physics.Raycast(ray, out hit, grabRange))
        {
            InteractableObject interactable = hit.collider.GetComponentInParent<InteractableObject>();
            if (interactable != null)
            {
                interactable.Highlight();
            }
        }
    }
}
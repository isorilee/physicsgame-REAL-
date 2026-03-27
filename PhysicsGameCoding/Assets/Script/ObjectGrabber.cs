using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectGrabber : MonoBehaviour
{
    [Tooltip("how far away the player can grab objects from")]
    public float grabRange = 4;

    [Tooltip("how fast the held object moves to the hold point. higher snappier")]
    public float holdSmoothing = 15f;

    //the point in front of the camera where the object is held 
    public Transform holdPoint;

    //how much force is applied when throwing
    public float throwForce = 15f;

    private Rigidbody heldObject;
    private bool isHolding = false;

    private InteractableObject currentHighlight;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void FixedUpdate()
    {
        //fixed update runs on an interval schedule 
        //we move the held object here so it stays smooth and physics it accurate 

        if (isHolding && heldObject != null) MoveHeldObject();
    }

    // Update is called once per frame
    void Update()
    {
        //runs the detection raycast every frame to update the highlight 
        //this is diff from grab raycast it just check what the player is looking at and highlights/unhighlights accordingly
        UpdateHighlight();
    }

    void TryGrab()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        //drawing the ray for debugging 
        Debug.DrawRay(transform.position, transform.forward * grabRange, Color.yellow, 0.5f);

        if (Physics.Raycast(ray, out hit, grabRange))
        {
            //check if the hit object has the interactable marker scripts 

            InteractableObject interactable = hit.collider.GetComponent<InteractableObject>();
            if (interactable != null)
            {
                //get rigidbody so we can move it with physics 
                heldObject = hit.collider.GetComponent<Rigidbody>();
                if (heldObject != null)
                {

                    //disable gravity so it floats in front of us while held 
                    heldObject.useGravity = false;

                    //freeze rotation so it doesnt spin around while carried 
                    heldObject.freezeRotation = true;

                    //zero out any existing velocity so it doesnt fly away 
                    heldObject.linearVelocity = Vector3.zero;
                    heldObject.angularVelocity = Vector3.zero;

                  
                    
                    
                    //unhighlight function here!!!!!!!! IMPORTANT 




                    isHolding = true; 
                    Debug.Log($"Grabbed {heldObject.name}");
                }


            }


        }


    }

    //update: runs every frame | fixed update runs at scheduled intervals 

    void MoveHeldObject()
    {
        Vector3 targetPos = holdPoint.position;
        Vector3 currentPos = heldObject.position; 

        //smoothly interpolate toward the hold point 
        //move position respects physics collison (object wont clip thru wall)

        Vector3 newPos = Vector3.Lerp (currentPos, targetPos, holdSmoothing * Time.fixedDeltaTime);

        heldObject.MovePosition(newPos); 


    }

    //drop
    //release the object and restores normal physics behavior 

    void DropObject()
    {
        if (heldObject == null) return; //if we arent holding anything, exit the function

        //re-enable gravity and rotation
        heldObject.useGravity = true;
        heldObject.freezeRotation = false;

        heldObject = null; //clearing it 
        isHolding = false;
        Debug.Log("Dropped object");

    }
    //release the obj and launches it forward using address

    void ThrowObject()
    {
        if (heldObject == null) return;

        //re-enable physics 
        heldObject.useGravity = true; 
        heldObject.freezeRotation = false;

        //apply force in the direction the camera is facing 
        //forcemode.impulse applies the force INSTANTLY like a punch 
        //as opposed to forcemode.force which applies gradually over time 
        heldObject.AddForce(transform.forward * throwForce, ForceMode.Impulse);

        heldObject = null;
        isHolding = false;
        Debug.Log("Threw Object");



    }

    public void OnGrabPerformed(InputAction.CallbackContext context)
    {
        if (isHolding) DropObject();
        else TryGrab();

    }

    public void OnThrowPerformed(InputAction.CallbackContext context)
    { 
        if(isHolding) ThrowObject();

    }
    
    void UpdateHighlight()
    {
        //dont change highlight while holding an object 
        if (isHolding) return; 


        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        Debug.DrawRay(transform.position, transform.forward * grabRange, Color.red); 

        if(Physics.Raycast(ray, out hit, grabRange))
        {
            //class means script, not a variable 
            InteractableObject interactable = hit.collider.GetComponent<InteractableObject>();
            if (interactable != null)

            if(currentHighlight != null && currentHighlight != interactable)
            {
                currentHighlight.Unhighlight();
                Debug.Log("unhiglighted");
            }

            //highlight the new obj 
            interactable.Highlight();
            Debug.Log("call highlight");
            currentHighlight = interactable;
            return; 

        }

        //raycast hits nothing interactable clear highlight 
        if(currentHighlight != null)
        {
            currentHighlight.Unhighlight();
            currentHighlight = null; 

        }

    }



}




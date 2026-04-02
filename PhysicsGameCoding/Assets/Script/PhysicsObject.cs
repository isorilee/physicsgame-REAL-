using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //how heavy the object is in KG affects how much force is needed to moving 
    [Range(0.1f, 100f)]
    public float mass = 1f;


    //linear drag how quickly the object slows down in the air, 0 = no draft, 10 = very sluggish 
    [Range(0f, 10f)]
    public float drag = 0.5f;

    //angulat drag how quickly spinning slows down
    [Range(0f, 10f)]
    public float angularDrag = 0.5f;

    [Header("Surface Properties")]
    //bounciness of ths surface 0 = no bounce, 1 = perfect bounce, requires physics material is on the collider 
    [Range(0f, 1f)]
    public float bounciness = 0f;
    [Range(0, 1f)]
    public float friction = 0.6f;

    [Header("Puzzle Properties")]
    //the effective weight used by pressure plate
    //defaults to mass but can be overwritten
    public float puzzleWeight = -1f;

    Rigidbody rb;
    PhysicsMaterial physMat;
    public bool isHeld = false;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        //call our function here 
    }
    //sets mass and drag directly 
    void ApplyRigidbodySettings()
    {
        rb.mass = mass;
        rb.linearDamping = drag;
        rb.angularDamping = angularDrag;
    }

    //physics material in unity control bounce and friction 
    //we create a physmat at runtime and assign it 

    void ApplySurfaceSettings()
    {
        physMat = new PhysicsMaterial(gameObject.name);
        physMat.bounciness = bounciness;
        physMat.dynamicFriction = friction;
        physMat.staticFriction = friction;

        //comebineMode.maximum means the higher friction of the two 
        //colliding object wins, good defaukt for solide objects 

        physMat.frictionCombine = PhysicsMaterialCombine.Average;
        physMat.bounceCombine = PhysicsMaterialCombine.Maximum;


        //assign the material to the collider 
        Collider col = GetComponent<Collider>();
        if (col != null)

        {
            col.material = physMat;

        }
    }

    //preview in editor 
    //when you change values in the inspector during play mode 
    //this makes it apply immediately without restarting 


    private void OnValidate()
    {
        //on validate runs in the editor whenever an inspector value changes 
        if(rb != null) ApplyRigidbodySettings();
    }


}

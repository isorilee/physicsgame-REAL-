using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class FPSplayer : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float runSpeed = 9f;
    public float jumpSpeed = 5f;
    public float jumpForce = 5f;
    private bool isRunning;
    private bool jumpReady;

    [Header("Camera")]
    public Transform cameraTransform;
    public float lookSensitivity = 100f;
    private float yaw;
    private float pitch;

    private Rigidbody rb;

    private Vector2 moveInput;
    private Vector2 lookInput;

    [Header("Grounding")]
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.5f;
    public float groundCheckDistance = 0.5f;
    public bool isGrounded;
    public Transform groundCheck;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        //freezerotation? 

        //optional lock cursor 
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        CameraLook();
        GroundCheck();
    }

    private void FixedUpdate()
    {
        float currentSpeed;
        if (isRunning)
        {
            currentSpeed = runSpeed;
        }
        else
        {
            currentSpeed = walkSpeed;
        }

        //match movement, multiplied by the direction by the input of the player by the speed 
        Vector3 move = transform.forward * moveInput.y * currentSpeed + transform.right * moveInput.x * currentSpeed;

        rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);

    //there's something wrong with this movement. Might be X,Y points in the code 
    //the movement code is not enough, manual scripting needs for WASD keys 
        
        if (jumpReady && isGrounded)
        {
            jumpReady = false;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
    //need jump later
        

    void CameraLook()
    {
        if (cameraTransform == null) return;
        float mouseX = lookInput.x * lookSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * lookSensitivity * Time.deltaTime;

        

        //horizontal movement
        yaw += mouseX;
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);

        //vertical rotation rotates the camera only 
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
       moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public  void OnJump(InputAction.CallbackContext context)
    { 
        if (context.performed) jumpReady = true; 
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        isRunning = context.ReadValueAsButton ();
    }

    private void GroundCheck()
    {
        if (groundCheck == null )
        {
            isGrounded = false;
            return;
        }


        //ground check should not be interfering with anything
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        isGrounded = Physics.SphereCast(groundCheck.position, groundCheckRadius, Vector3.down,
            out RaycastHit hit, groundCheckDistance, groundLayer, QueryTriggerInteraction.Ignore);


    }

    private void OnDrawGizmosSelected()
    {
        //visualize the end position of the spherecase 
        Vector3 end = groundCheck.position + Vector3.down * groundCheckDistance;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(end, groundCheckRadius);
    }

}

using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class FPSplayer : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float runSpeed = 9f;
    public float jumpForce = 5f;
    private bool isRunning;
    private bool jumpReady;

    [Header("Drunk Movement")]
    public bool useDrunkInput = true;
    public float drunkInputAmount = 0.35f;
    public float drunkInputSpeed = 2f;

    [Header("Camera")]
    public Transform cameraTransform;
    public float lookSensitivity = 100f;
    private float yaw;
    private float pitch;

    [Header("Grounding")]
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.5f;
    public float groundCheckDistance = 0.5f;
    public bool isGrounded;
    public Transform groundCheck;

    [Header("Interaction")]
    public float interactDistance = 3f;
    private InteractableObject currentInteractable;

    [Header("UI Prompt")]
    public GameObject interactPrompt;
    public TextMeshProUGUI promptText;

    private Rigidbody rb;
    private Vector2 moveInput;
    private Vector2 lookInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }
    }

    void Update()
    {
        CameraLook();
        GroundCheck();
        CheckForInteractable();
    }

    void FixedUpdate()
    {
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        Vector2 finalMoveInput = moveInput;

        if (DrunkManager.instance != null && DrunkManager.instance.isDrunk && useDrunkInput)
        {
            float drunkLevel = DrunkManager.instance.drunkLevel;

            float wobbleX = Mathf.Sin(Time.time * drunkInputSpeed) * drunkInputAmount * drunkLevel;
            float wobbleY = Mathf.Cos(Time.time * drunkInputSpeed * 1.3f) * drunkInputAmount * drunkLevel;

            finalMoveInput += new Vector2(wobbleX, wobbleY);
            finalMoveInput = Vector2.ClampMagnitude(finalMoveInput, 1.4f);
        }

        Vector3 move = transform.forward * finalMoveInput.y * currentSpeed
                     + transform.right * finalMoveInput.x * currentSpeed;

        rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);

        if (jumpReady && isGrounded)
        {
            jumpReady = false;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void CameraLook()
    {
        if (cameraTransform == null) return;

        float mouseX = lookInput.x * lookSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * lookSensitivity * Time.deltaTime;

        yaw += mouseX;
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);

        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void CheckForInteractable()
    {
        if (cameraTransform == null) return;

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        Debug.DrawRay(cameraTransform.position, cameraTransform.forward * interactDistance, Color.green);

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            InteractableObject interactable = hit.collider.GetComponentInParent<InteractableObject>();

            if (interactable != null)
            {
                if (currentInteractable != interactable)
                {
                    ClearCurrentInteractable();
                    currentInteractable = interactable;
                    currentInteractable.Highlight();
                }

                ShowPrompt(interactable);
                return;
            }
        }

        HidePrompt();
        ClearCurrentInteractable();
    }

    void ShowPrompt(InteractableObject interactable)
    {
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(true);
        }

        if (promptText != null)
        {
            if (interactable.isDrink)
            {
                promptText.text = "Press F to drink me!";
            }
            else
            {
                promptText.text = "Press F to interact";
            }
        }
    }

    void HidePrompt()
    {
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }
    }

    void ClearCurrentInteractable()
    {
        if (currentInteractable != null)
        {
            currentInteractable.Unhighlight();
            currentInteractable = null;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            jumpReady = true;
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        isRunning = context.ReadValueAsButton();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }

    void GroundCheck()
    {
        if (groundCheck == null)
        {
            isGrounded = false;
            return;
        }

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

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Vector3 end = groundCheck.position + Vector3.down * groundCheckDistance;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(end, groundCheckRadius);
        }

        if (cameraTransform != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(cameraTransform.position, cameraTransform.position + cameraTransform.forward * interactDistance);
        }
    }
}
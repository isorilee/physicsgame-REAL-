using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class FPSplayer : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float runSpeed = 9f;
    public float jumpForce = 5f;
    private bool isRunning;
    private bool jumpReady;

    [Header("Ragdoll Movement Target")]
    public Rigidbody ragdollHips;

    [Header("Anti-Flying")]
    public float groundedStickForce = 20f;
    public float maxHorizontalSpeed = 10f;
    public float maxVerticalSpeed = 12f;

    [Header("Controlled Tipping")]
    public bool allowTipping = true;
    public float angularDamping = 8f;
    public float maxAngularVelocity = 3f;

    [Header("Drunk Movement")]
    public bool useDrunkInput = true;
    public float drunkInputAmount = 0.35f;
    public float drunkInputSpeed = 2f;

    [Header("Camera")]
    public Transform cameraTransform;
    public float lookSensitivity = 100f;
    private float yaw;
    private float pitch;

    [Header("Intro Camera")]
    public float introLookDelay = 2f;

    [Header("Drink Sequence")]
    public bool canMove = false;
    public bool canLook = false;

    public GameObject firstPersonCine;
    public GameObject thirdPersonCine;

    [Header("Animation")]
    public Animator characterAnimator;
    public string walkingBoolName = "IsWalking";

    public float delayBeforeCameraSwitch = 1.5f;
    public float delayBeforeMovementUnlock = 1.5f;

    [Header("Grounding")]
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.5f;
    public float groundCheckDistance = 0.5f;
    public bool isGrounded;
    public Transform groundCheck;

    [Header("Interaction")]
    public float interactDistance = 8f;
    private InteractableObject currentInteractable;

    [Header("UI Prompt")]
    public GameObject interactPrompt;
    public TextMeshProUGUI promptText;

    [Header("Reticle")]
    public Image reticleImage;
    public Color normalReticleColor = Color.white;
    public Color interactReticleColor = Color.yellow;

    private Rigidbody rb;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool hasStartedDrinkSequence = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("FPSplayer needs a Rigidbody on the Player object.");
            enabled = false;
            return;
        }

        if (ragdollHips == null)
        {
            Debug.LogWarning("Ragdoll Hips is not assigned on FPSplayer.");
        }

        rb.freezeRotation = true;
        rb.angularDamping = angularDamping;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        if (ragdollHips != null)
        {
            ragdollHips.angularDamping = angularDamping;
            ragdollHips.interpolation = RigidbodyInterpolation.Interpolate;
            ragdollHips.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            ragdollHips.maxAngularVelocity = maxAngularVelocity;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        canMove = false;
        canLook = false;

        if (firstPersonCine != null)
            firstPersonCine.SetActive(true);

        if (thirdPersonCine != null)
            thirdPersonCine.SetActive(false);

        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        if (reticleImage != null)
            reticleImage.color = normalReticleColor;

        StartCoroutine(EnableLookAfterIntro());
    }

    private IEnumerator EnableLookAfterIntro()
    {
        yield return new WaitForSeconds(introLookDelay);
        canLook = true;
    }

    void Update()
    {
        if (canLook)
            CameraLook();

        GroundCheck();
        CheckForInteractable();
        UpdateWalkAnimation();
    }

    void FixedUpdate()
    {
        if (canMove)
        {
            MovePlayer();
        }
        else
        {
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);

            if (ragdollHips != null)
                ragdollHips.linearVelocity = new Vector3(0f, ragdollHips.linearVelocity.y, 0f);
        }

        LimitVelocityAndSpin();
    }

    void MovePlayer()
    {
        if (ragdollHips == null) return;

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

        Vector3 forward = cameraTransform != null ? cameraTransform.forward : transform.forward;
        Vector3 right = cameraTransform != null ? cameraTransform.right : transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 move = forward * finalMoveInput.y + right * finalMoveInput.x;
        move = Vector3.ClampMagnitude(move, 1f);

        Vector3 targetVelocity = move * currentSpeed;

        ragdollHips.linearVelocity = new Vector3(
            targetVelocity.x,
            ragdollHips.linearVelocity.y,
            targetVelocity.z
        );

        if (isGrounded && !jumpReady)
            ragdollHips.AddForce(Vector3.down * groundedStickForce, ForceMode.Acceleration);

        if (jumpReady && isGrounded)
        {
            jumpReady = false;

            ragdollHips.linearVelocity = new Vector3(
                ragdollHips.linearVelocity.x,
                0f,
                ragdollHips.linearVelocity.z
            );

            ragdollHips.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void LimitVelocityAndSpin()
    {
        if (ragdollHips == null) return;

        Vector3 velocity = ragdollHips.linearVelocity;
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0f, velocity.z);

        if (horizontalVelocity.magnitude > maxHorizontalSpeed)
            horizontalVelocity = horizontalVelocity.normalized * maxHorizontalSpeed;

        float clampedY = Mathf.Clamp(velocity.y, -maxVerticalSpeed, maxVerticalSpeed);

        ragdollHips.linearVelocity = new Vector3(
            horizontalVelocity.x,
            clampedY,
            horizontalVelocity.z
        );

        if (allowTipping)
        {
            ragdollHips.angularVelocity = Vector3.ClampMagnitude(
                ragdollHips.angularVelocity,
                maxAngularVelocity
            );
        }
    }

    void CameraLook()
    {
        if (cameraTransform == null) return;

        float mouseY = lookInput.y * lookSensitivity * Time.deltaTime;
        float mouseX = lookInput.x * lookSensitivity * Time.deltaTime;

        yaw += mouseX;
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);

        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void UpdateWalkAnimation()
    {
        if (characterAnimator == null) return;

        // canMove랑 상관없이 WASD / Move Input 들어오면 바로 Walking
        bool hasMoveInput = moveInput.magnitude > 0.1f;

        characterAnimator.SetBool(walkingBoolName, hasMoveInput);
    }

    public void StartDrinkStandSequence()
    {
        if (hasStartedDrinkSequence)
            return;

        hasStartedDrinkSequence = true;

        if (DrunkManager.instance != null)
            DrunkManager.instance.Drink();

        StartCoroutine(DrinkSequence());
    }

    private IEnumerator DrinkSequence()
    {
        canMove = false;
        canLook = true;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (ragdollHips != null)
        {
            ragdollHips.linearVelocity = Vector3.zero;
            ragdollHips.angularVelocity = Vector3.zero;
        }

        yield return new WaitForSeconds(delayBeforeCameraSwitch);

        if (firstPersonCine != null)
            firstPersonCine.SetActive(false);

        if (thirdPersonCine != null)
            thirdPersonCine.SetActive(true);

        yield return new WaitForSeconds(delayBeforeMovementUnlock);

        canMove = true;
        canLook = true;
    }

    void CheckForInteractable()
    {
        Camera cam = Camera.main;

        if (cam == null)
        {
            Debug.LogWarning("MainCamera Tag가 MainCamera인지 확인해.");
            return;
        }

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.red, 0.1f);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
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
            interactPrompt.SetActive(true);

        if (promptText != null)
            promptText.text = interactable.isDrink ? "Press F to drink me!" : "Press F to interact";

        if (reticleImage != null)
            reticleImage.color = interactReticleColor;
    }

    void HidePrompt()
    {
        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        if (reticleImage != null)
            reticleImage.color = normalReticleColor;
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
            jumpReady = true;
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        isRunning = context.ReadValueAsButton();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (currentInteractable != null)
            currentInteractable.Interact();
        else
            Debug.Log("No current interactable");
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
    }
}
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(CharacterController))]
public class InputSystemFPSController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraRoot;
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float sprintSpeed = 7f;
    [SerializeField] private float crouchSpeed = 2f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float jumpHeight = 1.2f;
    [Header("Look")]
    [SerializeField] private float mouseSensitivity = 0.08f;
    [SerializeField] private float minPitch = -85f;
    [SerializeField] private float maxPitch = 85f;
    [Header("Crouch")]
    [SerializeField] private float standingHeight = 1.8f;
    [SerializeField] private float crouchingHeight = 1.0f;
    [SerializeField] private float crouchTransitionSpeed = 10f;
    private CharacterController controller;
    private PlayerInputActions inputActions;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 verticalVelocity;
    private float pitch;
    private bool isSprinting;
    private bool isCrouching;
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        inputActions = new PlayerInputActions();
    }
    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;
        inputActions.Player.Look.performed += OnLook;
        inputActions.Player.Look.canceled += OnLook;
        inputActions.Player.Sprint.performed += OnSprintStarted;
        inputActions.Player.Sprint.canceled += OnSprintCanceled;
        inputActions.Player.Crouch.performed += OnCrouchStarted;
        inputActions.Player.Crouch.canceled += OnCrouchCanceled;
        inputActions.Player.Jump.performed += OnJump;
    }
    private void OnDisable()
    {
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMove;
        inputActions.Player.Look.performed -= OnLook;
        inputActions.Player.Look.canceled -= OnLook;
        inputActions.Player.Sprint.performed -= OnSprintStarted;
        inputActions.Player.Sprint.canceled -= OnSprintCanceled;
        inputActions.Player.Crouch.performed -= OnCrouchStarted;
        inputActions.Player.Crouch.canceled -= OnCrouchCanceled;
        inputActions.Player.Jump.performed -= OnJump;
        inputActions.Player.Disable();
    }
    private void Start()
    {
        LockCursor();
        controller.height = standingHeight;
        //controller.center = new Vector3(0f, standingHeight / 2f, 0f);
    }
    private void Update()
    {
        HandleLook();
        HandleMovement();
        HandleCrouch();
    }
    private void HandleLook()
    {
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;
        transform.Rotate(Vector3.up * mouseX);
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        cameraRoot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
    private void HandleMovement()
    {
        float currentSpeed = walkSpeed;
        if (isCrouching)
        {
            currentSpeed = crouchSpeed;
        }
        else if (isSprinting)
        {
            currentSpeed = sprintSpeed;
        }
        Vector3 moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);
        controller.Move(moveDirection * currentSpeed * Time.deltaTime);
        if (controller.isGrounded && verticalVelocity.y < 0f)
        {
            verticalVelocity.y = -2f;
        }
        verticalVelocity.y += gravity * Time.deltaTime;
        controller.Move(verticalVelocity * Time.deltaTime);
    }
    private void HandleCrouch()
    {
        float targetHeight = isCrouching ? crouchingHeight : standingHeight;
        controller.height = Mathf.Lerp(
            controller.height,
            targetHeight,
            crouchTransitionSpeed * Time.deltaTime
        );
        //controller.center = new Vector3(0f, controller.height / 2f, 0f);
        if (cameraRoot != null)
        {
            float targetCameraY = isCrouching ? 0.9f : 1.6f;
            Vector3 localPosition = cameraRoot.localPosition;
            localPosition.y = Mathf.Lerp(
                localPosition.y,
                targetCameraY,
                crouchTransitionSpeed * Time.deltaTime
            );
            cameraRoot.localPosition = localPosition;
        }
    }
    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
    private void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }
    private void OnSprintStarted(InputAction.CallbackContext context)
    {
        isSprinting = true;
    }
    private void OnSprintCanceled(InputAction.CallbackContext context)
    {
        isSprinting = false;
    }
    private void OnCrouchStarted(InputAction.CallbackContext context)
    {
        isCrouching = true;
    }
    private void OnCrouchCanceled(InputAction.CallbackContext context)
    {
        isCrouching = false;
    }
    private void OnJump(InputAction.CallbackContext context)
    {
        if (!controller.isGrounded || isCrouching)
        {
            return;
        }
        verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }
    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void SetLookEnabled(bool enabled)
    {
        if (enabled)
        {
            inputActions.Player.Look.Enable();
        }
        else
        {
            inputActions.Player.Look.Disable();
            lookInput = Vector2.zero;
        }
    }
}

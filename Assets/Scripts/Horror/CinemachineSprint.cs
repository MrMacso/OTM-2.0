using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CinemachineSprintFOV : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] private InputSystemFPSController playerController;

    [Header("FOV")]
    [Tooltip("Default first-person field of view. Raise this if the camera feels too zoomed in.")]
    [SerializeField] private float normalFOV = 75f;

    [Tooltip("Field of view while sprinting. Keep this only a little higher than normalFOV.")]
    [SerializeField] private float sprintFOV = 82f;

    [SerializeField] private float fovChangeSpeed = 8f;

    private PlayerInputActions inputActions;
    private bool isSprinting;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Sprint.performed += OnSprintStarted;
        inputActions.Player.Sprint.canceled += OnSprintCanceled;

        if (cinemachineCamera != null)
        {
            cinemachineCamera.Lens.FieldOfView = normalFOV;
        }
    }

    private void OnDisable()
    {
        inputActions.Player.Sprint.performed -= OnSprintStarted;
        inputActions.Player.Sprint.canceled -= OnSprintCanceled;
        inputActions.Player.Disable();
    }

    private void Update()
    {
        if (cinemachineCamera == null)
        {
            return;
        }

        float targetFOV = isSprinting ? sprintFOV : normalFOV;
        cinemachineCamera.Lens.FieldOfView = Mathf.Lerp(
            cinemachineCamera.Lens.FieldOfView,
            targetFOV,
            fovChangeSpeed * Time.deltaTime
        );
    }

    private void OnSprintStarted(InputAction.CallbackContext context)
    {
        isSprinting = true;
    }

    private void OnSprintCanceled(InputAction.CallbackContext context)
    {
        isSprinting = false;
    }
}

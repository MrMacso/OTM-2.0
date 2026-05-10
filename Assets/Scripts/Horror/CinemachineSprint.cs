using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
public class CinemachineSprintFOV : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] private InputSystemFPSController playerController;
    [Header("FOV")]
    [SerializeField] private float normalFOV = 60f;
    [SerializeField] private float sprintFOV = 72f;
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
    }
    private void OnDisable()
    {
        inputActions.Player.Sprint.performed -= OnSprintStarted;
        inputActions.Player.Sprint.canceled -= OnSprintCanceled;
        inputActions.Player.Disable();
    }
    private void Update()
    {
        if (cinemachineCamera == null) return;
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

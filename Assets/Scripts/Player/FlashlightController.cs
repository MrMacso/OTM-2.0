using UnityEngine;
using UnityEngine.InputSystem;

public class FlashlightController : MonoBehaviour
{
    [SerializeField] private Light flashlightLight;
    [SerializeField] private AudioSource toggleAudio;
    [SerializeField] private PlayerControlStateController playerControl;

    private PlayerInputActions inputActions;
    private bool isOn;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Flashlight.performed += OnToggleFlashlight;
    }

    private void OnDisable()
    {
        inputActions.Player.Flashlight.performed -= OnToggleFlashlight;
        inputActions.Player.Disable();
    }

    private void Start()
    {
        if (flashlightLight == null)
        {
            Debug.LogWarning($"{nameof(FlashlightController)} on {name} has no flashlight light assigned.");
            enabled = false;
            return;
        }

        flashlightLight.enabled = isOn;
    }

    private void OnToggleFlashlight(InputAction.CallbackContext context)
    {
        if (playerControl != null && playerControl.CurrentMode != PlayerControlMode.Gameplay)
        {
            return;
        }

        isOn = !isOn;
        flashlightLight.enabled = isOn;

        if (toggleAudio != null)
        {
            toggleAudio.Play();
        }
    }
}

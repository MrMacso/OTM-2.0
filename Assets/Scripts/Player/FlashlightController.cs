using UnityEngine;
using UnityEngine.InputSystem;
public class FlashlightController : MonoBehaviour
{
    [SerializeField] private Light flashlightLight;
    [SerializeField] private AudioSource toggleAudio;
    private PlayerInputActions inputActions;
    private bool isOn = false;
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
        if (flashlightLight != null)
        {
            flashlightLight.enabled = isOn;
        }
    }
    private void OnToggleFlashlight(InputAction.CallbackContext context)
    {
        isOn = !isOn;
        if (flashlightLight != null)
        {
            flashlightLight.enabled = isOn;
        }
        if (toggleAudio != null)
        {
            toggleAudio.Play();
        }
    }
}

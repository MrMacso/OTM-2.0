using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private LayerMask interactMask;
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI promptText;
    private PlayerInputActions inputActions;
    private IInteractable currentInteractable;
    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }
    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Interact.performed += OnInteract;
    }
    private void OnDisable()
    {
        inputActions.Player.Interact.performed -= OnInteract;
        inputActions.Player.Disable();
    }
    private void Update()
    {
        CheckForInteractable();
    }
    private void CheckForInteractable()
    {
        currentInteractable = null;
        if (promptText != null)
        {
            promptText.text = "";
        }
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactMask))
        {
            Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.red);
            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                currentInteractable = interactable;
                if (promptText != null)
                {
                    promptText.text = interactable.GetPrompt();
                }                
            }
        }
        if (Physics.Raycast(ray, out RaycastHit hitt, interactDistance))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.green);
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.red);
        }
    }
    private void OnInteract(InputAction.CallbackContext context)
    {
        currentInteractable?.Interact(this);
    }
}
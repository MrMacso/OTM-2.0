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

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
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
        ClearCurrentInteractable();
    }

    private void Update()
    {
        CheckForInteractable();
    }

    private void CheckForInteractable()
    {
        ClearCurrentInteractable();

        if (playerCamera == null)
        {
            return;
        }

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        int mask = interactMask.value == 0 ? Physics.DefaultRaycastLayers : interactMask.value;

        if (!Physics.Raycast(ray, out RaycastHit hit, interactDistance, mask, QueryTriggerInteraction.Collide))
        {
            Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.red);
            return;
        }

        Debug.DrawLine(ray.origin, hit.point, Color.green);

        currentInteractable = hit.collider.GetComponentInParent<IInteractable>();

        if (currentInteractable != null && promptText != null)
        {
            promptText.text = currentInteractable.InteractionPrompt;
        }
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        currentInteractable?.Interact(gameObject);
    }

    private void ClearCurrentInteractable()
    {
        currentInteractable = null;

        if (promptText != null)
        {
            promptText.text = string.Empty;
        }
    }
}

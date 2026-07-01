using System.Text;
using UnityEngine;
using UnityEngine.Events;

public class PocketWatchAssemblyStation : MonoBehaviour, IInteractable
{
    [Header("Interaction")]
    [SerializeField] private string prompt = "Press E to assemble the pocket watch";
    [SerializeField] private string completedPrompt = "The pocket watch is assembled.";

    [Header("Required Items")]
    [SerializeField] private InventoryItemRequirement[] requiredParts;
    [SerializeField] private bool consumeRequiredParts = true;

    [Header("Output")]
    [SerializeField] private InventoryItemDefinition completedWatchItem;
    [SerializeField] private int completedWatchQuantity = 1;

    [Header("Progress")]
    [SerializeField] private ProgressFlagReference assembledFlag = new ProgressFlagReference(ProgressFlags.PocketWatchAssembled);
    [SerializeField] private ProgressFlagReference pocketWatchCollectedFlag = new ProgressFlagReference(ProgressFlags.PocketWatchCollected);

    [Header("Feedback")]
    [SerializeField] private string missingPartsHeader = "The watch is incomplete.";
    [SerializeField] private string assembledFeedback = "The pocket watch ticks once, then falls silent.";
    [SerializeField] private bool listMissingParts = true;

    [Header("Scene References")]
    [SerializeField] private PocketWatchInteractable pocketWatchInteractable;
    [SerializeField] private GameObject incompleteVisual;
    [SerializeField] private GameObject completedVisual;

    [Header("Events")]
    [SerializeField] private UnityEvent onMissingParts;
    [SerializeField] private UnityEvent onAssembled;

    private bool isAssembled;

    public string InteractionPrompt => isAssembled ? completedPrompt : prompt;

    private void Start()
    {
        RestoreProgressState();
        ApplyVisualState();
    }

    public void Interact(GameObject interactor)
    {
        TryAssemble();
    }

    public bool TryAssemble()
    {
        if (isAssembled)
        {
            return false;
        }

        if (!HasAllRequiredParts())
        {
            FeedbackMessageUI.Instance?.ShowWarning(BuildMissingPartsMessage());
            onMissingParts?.Invoke();
            return false;
        }

        if (consumeRequiredParts)
        {
            ConsumeRequiredParts();
        }

        if (completedWatchItem != null)
        {
            Inventory.Instance?.AddItem(completedWatchItem, Mathf.Max(1, completedWatchQuantity));
        }

        if (assembledFlag.IsAssigned)
        {
            GameProgressManager.Instance?.AddProgressFlag(assembledFlag);
        }

        if (pocketWatchCollectedFlag.IsAssigned)
        {
            GameProgressManager.Instance?.AddProgressFlag(pocketWatchCollectedFlag);
        }

        if (pocketWatchInteractable == null)
        {
            pocketWatchInteractable = FindFirstObjectByType<PocketWatchInteractable>();
        }

        pocketWatchInteractable?.BindWatch();

        isAssembled = true;
        ApplyVisualState();
        FeedbackMessageUI.Instance?.ShowDiscovery(assembledFeedback);
        onAssembled?.Invoke();
        return true;
    }

    private void RestoreProgressState()
    {
        if (assembledFlag.IsAssigned && GameProgressManager.Instance != null)
        {
            isAssembled = GameProgressManager.Instance.HasProgressFlag(assembledFlag);
        }
    }

    private bool HasAllRequiredParts()
    {
        if (requiredParts == null || requiredParts.Length == 0)
        {
            return true;
        }

        if (Inventory.Instance == null)
        {
            return false;
        }

        foreach (InventoryItemRequirement requirement in requiredParts)
        {
            if (requirement == null || requirement.item == null)
            {
                continue;
            }

            if (!Inventory.Instance.HasItem(requirement.item, Mathf.Max(1, requirement.quantity)))
            {
                return false;
            }
        }

        return true;
    }

    private void ConsumeRequiredParts()
    {
        if (requiredParts == null || Inventory.Instance == null)
        {
            return;
        }

        foreach (InventoryItemRequirement requirement in requiredParts)
        {
            if (requirement == null || requirement.item == null)
            {
                continue;
            }

            Inventory.Instance.RemoveItem(requirement.item, Mathf.Max(1, requirement.quantity));
        }
    }

    private string BuildMissingPartsMessage()
    {
        if (!listMissingParts || requiredParts == null || Inventory.Instance == null)
        {
            return missingPartsHeader;
        }

        StringBuilder builder = new StringBuilder(missingPartsHeader);

        foreach (InventoryItemRequirement requirement in requiredParts)
        {
            if (requirement == null || requirement.item == null)
            {
                continue;
            }

            int requiredQuantity = Mathf.Max(1, requirement.quantity);
            int currentQuantity = Inventory.Instance.GetQuantity(requirement.item);

            if (currentQuantity < requiredQuantity)
            {
                builder.AppendLine();
                builder.Append("Missing: ");
                builder.Append(requirement.item.DisplayName);

                if (requiredQuantity > 1)
                {
                    builder.Append($" ({currentQuantity}/{requiredQuantity})");
                }
            }
        }

        return builder.ToString();
    }

    private void ApplyVisualState()
    {
        if (incompleteVisual != null)
        {
            incompleteVisual.SetActive(!isAssembled);
        }

        if (completedVisual != null)
        {
            completedVisual.SetActive(isAssembled);
        }
    }
}

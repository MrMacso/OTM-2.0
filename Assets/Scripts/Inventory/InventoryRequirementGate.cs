using UnityEngine;
using UnityEngine.Events;

public class InventoryRequirementGate : MonoBehaviour, IInteractable
{
    [Header("Interaction")]
    [SerializeField] private string prompt = "Press E to use item";
    [SerializeField] private bool canInteractDirectly = true;

    [Header("Requirements")]
    [SerializeField] private InventoryItemRequirement[] requiredItems;
    [SerializeField] private bool consumeRequiredItems;

    [Header("Feedback")]
    [SerializeField] private string missingItemFeedback = "You do not have the required item.";
    [SerializeField] private string successFeedback = "That worked.";

    [Header("Progress")]
    [SerializeField] private ProgressFlagReference progressFlagOnSuccess;

    [Header("Events")]
    [SerializeField] private UnityEvent onRequirementsMet;
    [SerializeField] private UnityEvent onRequirementsMissing;

    public string InteractionPrompt => prompt;

    public void Interact(GameObject interactor)
    {
        if (!canInteractDirectly)
        {
            return;
        }

        TryUseRequirements();
    }

    public bool TryUseRequirements()
    {
        if (!HasRequirements())
        {
            FeedbackMessageUI.Instance?.ShowWarning(missingItemFeedback);
            onRequirementsMissing?.Invoke();
            return false;
        }

        if (consumeRequiredItems)
        {
            ConsumeRequirements();
        }

        if (progressFlagOnSuccess.IsAssigned)
        {
            GameProgressManager.Instance?.AddProgressFlag(progressFlagOnSuccess);
        }

        FeedbackMessageUI.Instance?.ShowMessage(successFeedback);
        onRequirementsMet?.Invoke();
        return true;
    }

    public bool HasRequirements()
    {
        if (requiredItems == null || requiredItems.Length == 0)
        {
            return true;
        }

        if (Inventory.Instance == null)
        {
            return false;
        }

        foreach (InventoryItemRequirement requirement in requiredItems)
        {
            if (requirement == null || requirement.item == null)
            {
                continue;
            }

            int quantity = Mathf.Max(1, requirement.quantity);

            if (!Inventory.Instance.HasItem(requirement.item, quantity))
            {
                return false;
            }
        }

        return true;
    }

    private void ConsumeRequirements()
    {
        if (requiredItems == null || Inventory.Instance == null)
        {
            return;
        }

        foreach (InventoryItemRequirement requirement in requiredItems)
        {
            if (requirement == null || requirement.item == null)
            {
                continue;
            }

            Inventory.Instance.RemoveItem(requirement.item, Mathf.Max(1, requirement.quantity));
        }
    }
}

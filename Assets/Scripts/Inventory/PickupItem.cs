using UnityEngine;
using UnityEngine.Events;

public class PickupItem : MonoBehaviour, IInteractable
{
    [Header("Item")]
    [SerializeField] private InventoryItemDefinition itemDefinition;
    [SerializeField] private InventoryItem item;
    [SerializeField] private int quantity = 1;

    [Header("Interaction")]
    [SerializeField] private string promptPrefix = "Press E to pick up ";
    [SerializeField] private bool destroyAfterPickup = true;

    [Header("Feedback")]
    [SerializeField] private string pickupFeedbackOverride;

    [Header("Events")]
    [SerializeField] private UnityEvent onPickedUp;

    public string InteractionPrompt => promptPrefix + GetItemName();

    public void Interact(GameObject player)
    {
        if (Inventory.Instance == null)
        {
            Debug.LogWarning("No Inventory found in scene.");
            return;
        }

        bool wasAdded = false;

        if (itemDefinition != null)
        {
            wasAdded = Inventory.Instance.AddItem(itemDefinition, quantity);
        }
        else if (item != null)
        {
            Inventory.Instance.AddItem(item);
            wasAdded = true;
        }
        else
        {
            Debug.LogWarning($"{nameof(PickupItem)} on {name} has no item assigned.");
            return;
        }

        if (!wasAdded)
        {
            return;
        }

        string message = string.IsNullOrWhiteSpace(pickupFeedbackOverride)
            ? $"Picked up {GetItemName()}."
            : pickupFeedbackOverride;
        FeedbackMessageUI.Instance?.ShowDiscovery(message);

        onPickedUp?.Invoke();

        if (destroyAfterPickup)
        {
            Destroy(gameObject);
        }
    }

    private string GetItemName()
    {
        if (itemDefinition != null)
        {
            return itemDefinition.DisplayName;
        }

        if (item != null)
        {
            return item.itemName;
        }

        return "item";
    }
}

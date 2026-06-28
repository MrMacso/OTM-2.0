using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable
{
    [SerializeField] private InventoryItem item;

    public string InteractionPrompt => item != null ? $"Press E to pick up {item.itemName}" : "Press E to pick up item";

    public void Interact(GameObject player)
    {
        if (item == null)
        {
            Debug.LogWarning($"{nameof(PickupItem)} on {name} has no item assigned.");
            return;
        }

        if (Inventory.Instance == null)
        {
            Debug.LogWarning("No Inventory found in scene.");
            return;
        }

        Inventory.Instance.AddItem(item);
        Destroy(gameObject);
    }
}

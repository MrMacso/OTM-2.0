using UnityEngine;
public class PickupItem : MonoBehaviour, IInteractable
{
    [SerializeField] private InventoryItem item;
    public string GetPrompt()
    {
        return $"Press E to pick up {item.itemName}";
    }
    public void Interact(PlayerInteraction player)
    {
        Inventory.Instance.AddItem(item);
        Destroy(gameObject);
    }
}

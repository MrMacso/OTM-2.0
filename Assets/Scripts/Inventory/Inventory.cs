using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    private readonly List<InventoryItem> items = new();

    public IReadOnlyList<InventoryItem> Items => items;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void AddItem(InventoryItem item)
    {
        if (item == null)
        {
            Debug.LogWarning("Tried to add a null inventory item.");
            return;
        }

        if (items.Contains(item))
        {
            return;
        }

        items.Add(item);
        Debug.Log("Picked up: " + item.itemName);

        if (item.isKeyItem && item.name.IndexOf("Pocket", System.StringComparison.OrdinalIgnoreCase) >= 0)
        {
            GameProgressManager.Instance?.AddProgressFlag(ProgressFlags.PocketWatchCollected);
        }
    }

    public bool HasItem(InventoryItem item)
    {
        return item != null && items.Contains(item);
    }
}

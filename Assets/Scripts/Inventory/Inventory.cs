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
        if (item == null) return;
        items.Add(item);
        Debug.Log("Picked up: " + item.itemName);
    }
    public bool HasItem(InventoryItem item)
    {
        return items.Contains(item);
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    [Header("Compatibility")]
    [SerializeField] private ProgressFlagReference pocketWatchCollectedFlag = new ProgressFlagReference(ProgressFlags.PocketWatchCollected);

    private readonly List<InventoryEntry> entries = new();
    private readonly List<InventoryItem> legacyItems = new();

    public IReadOnlyList<InventoryEntry> Entries => entries;
    public IReadOnlyList<InventoryItem> Items => legacyItems;

    public event Action<InventoryItemDefinition, int> ItemAdded;
    public event Action<InventoryItemDefinition, int> ItemRemoved;
    public event Action InventoryChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public bool AddItem(InventoryItemDefinition item, int quantity = 1)
    {
        if (item == null)
        {
            Debug.LogWarning("Tried to add a null inventory item definition.");
            return false;
        }

        if (quantity <= 0)
        {
            Debug.LogWarning($"Tried to add invalid quantity {quantity} for {item.name}.");
            return false;
        }

        InventoryEntry entry = FindEntry(item);

        if (entry != null)
        {
            if (item.IsUnique)
            {
                return false;
            }

            entry.quantity += quantity;
        }
        else
        {
            entry = new InventoryEntry(item, quantity);
            entries.Add(entry);
        }

        Debug.Log($"Picked up: {item.DisplayName}");

        if (item.ProgressFlagOnPickup.IsAssigned)
        {
            GameProgressManager.Instance?.AddProgressFlag(item.ProgressFlagOnPickup);
        }

        ItemAdded?.Invoke(item, quantity);
        InventoryChanged?.Invoke();
        return true;
    }

    public void AddItem(InventoryItem item)
    {
        if (item == null)
        {
            Debug.LogWarning("Tried to add a null legacy inventory item.");
            return;
        }

        if (legacyItems.Contains(item))
        {
            return;
        }

        legacyItems.Add(item);
        Debug.Log("Picked up: " + item.itemName);

        if (IsPocketWatch(item))
        {
            if (pocketWatchCollectedFlag.IsAssigned)
            {
                GameProgressManager.Instance?.AddProgressFlag(pocketWatchCollectedFlag);
            }

            PocketWatchInteractable sceneWatch = FindFirstObjectByType<PocketWatchInteractable>();
            sceneWatch?.BindWatch();
        }

        InventoryChanged?.Invoke();
    }

    public bool RemoveItem(InventoryItemDefinition item, int quantity = 1)
    {
        InventoryEntry entry = FindEntry(item);

        if (entry == null || quantity <= 0 || entry.quantity < quantity)
        {
            return false;
        }

        entry.quantity -= quantity;

        if (entry.quantity <= 0)
        {
            entries.Remove(entry);
        }

        ItemRemoved?.Invoke(item, quantity);
        InventoryChanged?.Invoke();
        return true;
    }

    public bool HasItem(InventoryItemDefinition item, int quantity = 1)
    {
        InventoryEntry entry = FindEntry(item);
        return entry != null && entry.quantity >= quantity;
    }

    public bool HasItem(InventoryItem item)
    {
        return item != null && legacyItems.Contains(item);
    }

    public int GetQuantity(InventoryItemDefinition item)
    {
        InventoryEntry entry = FindEntry(item);
        return entry != null ? entry.quantity : 0;
    }

    private InventoryEntry FindEntry(InventoryItemDefinition item)
    {
        if (item == null)
        {
            return null;
        }

        foreach (InventoryEntry entry in entries)
        {
            if (entry != null && entry.item == item)
            {
                return entry;
            }
        }

        return null;
    }

    private bool IsPocketWatch(InventoryItem item)
    {
        if (!item.isKeyItem)
        {
            return false;
        }

        string idSource = string.IsNullOrWhiteSpace(item.itemName) ? item.name : item.itemName;
        return idSource.IndexOf("Pocket", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
               idSource.IndexOf("Watch", System.StringComparison.OrdinalIgnoreCase) >= 0;
    }
}

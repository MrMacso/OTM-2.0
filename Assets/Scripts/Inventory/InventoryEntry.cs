using System;

[Serializable]
public class InventoryEntry
{
    public InventoryItemDefinition item;
    public int quantity;

    public InventoryEntry(InventoryItemDefinition item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }
}

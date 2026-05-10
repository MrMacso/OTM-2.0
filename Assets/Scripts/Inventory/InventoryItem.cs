using UnityEngine;
[CreateAssetMenu(menuName = "Horror Game/Inventory Item")]
public class InventoryItem : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    [TextArea] public string description;
    public bool isKeyItem;
}

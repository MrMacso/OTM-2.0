using UnityEngine;

[CreateAssetMenu(
    fileName = "InventoryItemDefinition",
    menuName = "Article 1944/Inventory Item")]
public class InventoryItemDefinition : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string itemId;
    [SerializeField] private string displayName;
    [SerializeField] private InventoryItemCategory category = InventoryItemCategory.Misc;

    [Header("Presentation")]
    [SerializeField] private Sprite icon;
    [TextArea]
    [SerializeField] private string description;
    [TextArea]
    [SerializeField] private string readableText;

    [Header("Rules")]
    [SerializeField] private bool isUnique = true;
    [SerializeField] private bool canBeUsed = false;
    [SerializeField] private bool canBeConsumed = false;

    [Header("Progress")]
    [SerializeField] private ProgressFlagReference progressFlagOnPickup;

    public string ItemId => string.IsNullOrWhiteSpace(itemId) ? name : itemId;
    public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? name : displayName;
    public InventoryItemCategory Category => category;
    public Sprite Icon => icon;
    public string Description => description;
    public string ReadableText => readableText;
    public bool IsUnique => isUnique;
    public bool CanBeUsed => canBeUsed;
    public bool CanBeConsumed => canBeConsumed;
    public ProgressFlagReference ProgressFlagOnPickup => progressFlagOnPickup;
}

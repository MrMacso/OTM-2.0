#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class InventoryItemAssetCreator
{
    private const string FolderPath = "Assets/Data/InventoryItems";

    private struct DefaultItemData
    {
        public readonly string Id;
        public readonly string DisplayName;
        public readonly InventoryItemCategory Category;
        public readonly string Description;
        public readonly string ReadableText;
        public readonly bool IsUnique;
        public readonly bool CanBeUsed;
        public readonly bool CanBeConsumed;
        public readonly string ProgressFlagOnPickup;

        public DefaultItemData(
            string id,
            string displayName,
            InventoryItemCategory category,
            string description,
            string readableText,
            bool isUnique,
            bool canBeUsed,
            bool canBeConsumed,
            string progressFlagOnPickup = null)
        {
            Id = id;
            DisplayName = displayName;
            Category = category;
            Description = description;
            ReadableText = readableText;
            IsUnique = isUnique;
            CanBeUsed = canBeUsed;
            CanBeConsumed = canBeConsumed;
            ProgressFlagOnPickup = progressFlagOnPickup;
        }
    }

    private static readonly DefaultItemData[] DefaultItems =
    {
        new DefaultItemData("watch_assembly_instruction", "Pocket Watch Assembly Instructions", InventoryItemCategory.Document, "A fragile page showing how the antique watch fits together.", "The balance wheel, crown, hands, and casing must be aligned before winding. A warning is written below: do not wind it near a place of death.", true, false, false),
        new DefaultItemData("watch_case", "Pocket Watch Case", InventoryItemCategory.WatchPart, "The outer shell of an antique pocket watch.", string.Empty, true, false, false),
        new DefaultItemData("watch_crown", "Pocket Watch Crown", InventoryItemCategory.WatchPart, "The winding crown from an antique pocket watch.", string.Empty, true, false, false),
        new DefaultItemData("watch_hands", "Pocket Watch Hands", InventoryItemCategory.WatchPart, "Thin watch hands, darkened with age.", string.Empty, true, false, false),
        new DefaultItemData("watch_balance_wheel", "Pocket Watch Balance Wheel", InventoryItemCategory.WatchPart, "A delicate balance wheel from the watch movement.", string.Empty, true, false, false),
        new DefaultItemData("completed_pocket_watch", "Completed Pocket Watch", InventoryItemCategory.KeyItem, "The assembled antique pocket watch.", string.Empty, true, true, false, ProgressFlags.PocketWatchCollected),
        new DefaultItemData("cassette_key", "Cassette Box Key", InventoryItemCategory.PuzzleItem, "A small key stolen from the past.", string.Empty, true, false, false, ProgressFlags.CassetteKeyStolenPast),
        new DefaultItemData("cassette_evidence", "Cassette Evidence", InventoryItemCategory.Evidence, "A name, a date, and a room number.", "The handwriting is rushed. One name is circled twice, beside a basement room number.", true, false, false, ProgressFlags.CassetteEvidenceFound),
        new DefaultItemData("sedative", "Sedative", InventoryItemCategory.Medicine, "A small dose that can help control panic.", string.Empty, false, true, true),
    };

    [MenuItem("Tools/Article 1944/Create Default Inventory Item Assets")]
    public static void CreateDefaultInventoryItems()
    {
        EnsureFolder(FolderPath);

        HashSet<string> existingItemIds = LoadExistingItemIds();
        int createdCount = 0;

        foreach (DefaultItemData itemData in DefaultItems)
        {
            if (existingItemIds.Contains(itemData.Id))
            {
                continue;
            }

            InventoryItemDefinition item = ScriptableObject.CreateInstance<InventoryItemDefinition>();
            SerializedObject serializedItem = new SerializedObject(item);
            serializedItem.FindProperty("itemId").stringValue = itemData.Id;
            serializedItem.FindProperty("displayName").stringValue = itemData.DisplayName;
            serializedItem.FindProperty("category").enumValueIndex = (int)itemData.Category;
            serializedItem.FindProperty("description").stringValue = itemData.Description;
            serializedItem.FindProperty("readableText").stringValue = itemData.ReadableText;
            serializedItem.FindProperty("isUnique").boolValue = itemData.IsUnique;
            serializedItem.FindProperty("canBeUsed").boolValue = itemData.CanBeUsed;
            serializedItem.FindProperty("canBeConsumed").boolValue = itemData.CanBeConsumed;
            serializedItem.FindProperty("progressFlagOnPickup").FindPropertyRelative("legacyFlagId").stringValue = itemData.ProgressFlagOnPickup ?? string.Empty;
            serializedItem.ApplyModifiedPropertiesWithoutUndo();

            string assetPath = AssetDatabase.GenerateUniqueAssetPath($"{FolderPath}/{ToAssetName(itemData.Id)}.asset");
            AssetDatabase.CreateAsset(item, assetPath);
            createdCount++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Inventory item asset generation complete. Created {createdCount} new asset(s).");
    }

    private static HashSet<string> LoadExistingItemIds()
    {
        HashSet<string> ids = new HashSet<string>();
        string[] guids = AssetDatabase.FindAssets("t:InventoryItemDefinition");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            InventoryItemDefinition item = AssetDatabase.LoadAssetAtPath<InventoryItemDefinition>(path);

            if (item != null && !string.IsNullOrWhiteSpace(item.ItemId))
            {
                ids.Add(item.ItemId);
            }
        }

        return ids;
    }

    private static void EnsureFolder(string folderPath)
    {
        string[] parts = folderPath.Split('/');
        string current = parts[0];

        for (int i = 1; i < parts.Length; i++)
        {
            string next = current + "/" + parts[i];

            if (!AssetDatabase.IsValidFolder(next))
            {
                AssetDatabase.CreateFolder(current, parts[i]);
            }

            current = next;
        }
    }

    private static string ToAssetName(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return "InventoryItem";
        }

        string[] parts = id.Split('_');

        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i].Length == 0)
            {
                continue;
            }

            parts[i] = char.ToUpperInvariant(parts[i][0]) + parts[i].Substring(1);
        }

        return string.Join(string.Empty, parts);
    }
}
#endif

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class ProgressFlagAssetCreator
{
    private const string FolderPath = "Assets/Data/ProgressFlags";

    private struct DefaultFlagData
    {
        public readonly string Id;
        public readonly string DisplayName;
        public readonly string Description;

        public DefaultFlagData(string id, string displayName, string description)
        {
            Id = id;
            DisplayName = displayName;
            Description = description;
        }
    }

    private static readonly DefaultFlagData[] DefaultFlags =
    {
        new DefaultFlagData(ProgressFlags.IntroVoiceFinished, "Intro Voice Finished", "The opening narration or intro beat has finished."),
        new DefaultFlagData(ProgressFlags.PocketWatchCollected, "Pocket Watch Collected", "The player has found or assembled the pocket watch."),
        new DefaultFlagData(ProgressFlags.PocketWatchAssembled, "Pocket Watch Assembled", "The player assembled the pocket watch from its missing parts."),
        new DefaultFlagData(ProgressFlags.FirstTimeTravelCompleted, "First Time Travel Completed", "The player has travelled away from the present for the first time."),
        new DefaultFlagData(ProgressFlags.CassetteKeyStolenPast, "Cassette Key Stolen In 1944", "The player stole the cassette box key in the past."),
        new DefaultFlagData(ProgressFlags.CassetteBoxOpenedPresent, "Cassette Box Opened In Present", "The cassette box has opened in the present due to a past action."),
        new DefaultFlagData(ProgressFlags.CassetteEvidenceFound, "Cassette Evidence Found", "The player collected the first evidence from the cassette box."),
        new DefaultFlagData(ProgressFlags.MummyNoteFound, "Mummy Note Found", "Legacy prototype flag for an early note clue."),
        new DefaultFlagData(ProgressFlags.BasementPuzzleSolved, "Basement Puzzle Solved", "The current first puzzle chain has been completed."),
        new DefaultFlagData(ProgressFlags.SecurityRoomUnlocked, "Security Room Unlocked", "The security room is unlocked."),
        new DefaultFlagData(ProgressFlags.FirstJumpscareTriggered, "First Jumpscare Triggered", "The first scripted horror beat has fired."),
        new DefaultFlagData(ProgressFlags.FirstBreathingRecovered, "First Breathing Recovery", "The player recovered from panic in a hiding spot."),
        new DefaultFlagData(ProgressFlags.DemoCompleted, "Demo Completed", "The current vertical slice has reached its end."),
    };

    [MenuItem("Tools/Article 1944/Create Default Progress Flag Assets")]
    public static void CreateDefaultProgressFlags()
    {
        EnsureFolder(FolderPath);

        HashSet<string> existingFlagIds = LoadExistingFlagIds();
        int createdCount = 0;

        foreach (DefaultFlagData flagData in DefaultFlags)
        {
            if (existingFlagIds.Contains(flagData.Id))
            {
                continue;
            }

            ProgressFlagDefinition flag = ScriptableObject.CreateInstance<ProgressFlagDefinition>();
            SerializedObject serializedFlag = new SerializedObject(flag);
            serializedFlag.FindProperty("flagId").stringValue = flagData.Id;
            serializedFlag.FindProperty("displayName").stringValue = flagData.DisplayName;
            serializedFlag.FindProperty("description").stringValue = flagData.Description;
            serializedFlag.ApplyModifiedPropertiesWithoutUndo();

            string assetPath = AssetDatabase.GenerateUniqueAssetPath($"{FolderPath}/{ToAssetName(flagData.Id)}.asset");
            AssetDatabase.CreateAsset(flag, assetPath);
            createdCount++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Progress flag asset generation complete. Created {createdCount} new asset(s).");
    }

    private static HashSet<string> LoadExistingFlagIds()
    {
        HashSet<string> ids = new HashSet<string>();
        string[] guids = AssetDatabase.FindAssets("t:ProgressFlagDefinition");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ProgressFlagDefinition flag = AssetDatabase.LoadAssetAtPath<ProgressFlagDefinition>(path);

            if (flag != null && !string.IsNullOrWhiteSpace(flag.FlagId))
            {
                ids.Add(flag.FlagId);
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
            return "ProgressFlag";
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

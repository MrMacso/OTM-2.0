using System;
using UnityEngine;

[CreateAssetMenu(
    fileName = "ProgressFlagDefinition",
    menuName = "Article 1944/Progress Flag")]
public class ProgressFlagDefinition : ScriptableObject
{
    [SerializeField] private string flagId;
    [SerializeField] private string displayName;
    [TextArea]
    [SerializeField] private string description;

    public string FlagId => string.IsNullOrWhiteSpace(flagId) ? name : flagId;
    public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? name : displayName;
    public string Description => description;
}

[Serializable]
public struct ProgressFlagReference
{
    [SerializeField] private ProgressFlagDefinition flag;
    [SerializeField] private string legacyFlagId;

    public ProgressFlagReference(string legacyFlagId)
    {
        flag = null;
        this.legacyFlagId = legacyFlagId;
    }

    public ProgressFlagDefinition Definition => flag;
    public string Id => flag != null ? flag.FlagId : legacyFlagId;
    public bool IsAssigned => flag != null || !string.IsNullOrWhiteSpace(legacyFlagId);

    public override string ToString()
    {
        return Id;
    }
}

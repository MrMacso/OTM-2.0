using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "GameStageDefinition",
    menuName = "Horror Game/Game Stage Definition")]
public class GameStageDefinition : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string stageId;
    [SerializeField] private string displayName;

    [Header("Requirements")]
    [SerializeField] private ProgressFlagReference[] requiredProgressFlags = Array.Empty<ProgressFlagReference>();

    public string StageId => stageId;
    public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? name : displayName;
    public IReadOnlyList<ProgressFlagReference> RequiredProgressFlags => requiredProgressFlags ?? Array.Empty<ProgressFlagReference>();
}

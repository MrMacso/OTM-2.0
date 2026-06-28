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
    [SerializeField] private string[] requiredProgressFlags = Array.Empty<string>();

    public string StageId => stageId;
    public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? name : displayName;
    public IReadOnlyList<string> RequiredProgressFlags => requiredProgressFlags ?? Array.Empty<string>();
}

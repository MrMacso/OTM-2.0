using System;
using System.Collections.Generic;
using UnityEngine;

public class GameProgressManager : MonoBehaviour
{
    public static GameProgressManager Instance { get; private set; }

    [Header("Stages")]
    [SerializeField] private GameStageDefinition initialStage;
    [SerializeField] private List<GameStageDefinition> allStages = new();

    private readonly HashSet<string> completedProgressFlagIds = new();

    public GameStageDefinition CurrentStage { get; private set; }

    public event Action<GameStageDefinition> StageChanged;
    public event Action<string> ProgressFlagAdded;
    public event Action<ProgressFlagDefinition> ProgressFlagDefinitionAdded;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Duplicate GameProgressManager found. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (initialStage != null)
        {
            CurrentStage = initialStage;
        }
        else
        {
            Debug.LogWarning("GameProgressManager has no initial stage assigned.");
        }
    }

    private void Start()
    {
        if (CurrentStage != null)
        {
            StageChanged?.Invoke(CurrentStage);
        }
    }

    public bool HasProgressFlag(ProgressFlagDefinition progressFlag)
    {
        return progressFlag != null && HasProgressFlag(progressFlag.FlagId);
    }

    public bool HasProgressFlag(ProgressFlagReference progressFlag)
    {
        return progressFlag.IsAssigned && HasProgressFlag(progressFlag.Id);
    }

    public bool HasProgressFlag(string progressFlagId)
    {
        if (string.IsNullOrWhiteSpace(progressFlagId))
        {
            return false;
        }

        return completedProgressFlagIds.Contains(progressFlagId);
    }

    public void AddProgressFlag(ProgressFlagDefinition progressFlag)
    {
        if (progressFlag == null)
        {
            Debug.LogWarning("Tried to add a null progress flag asset.");
            return;
        }

        AddProgressFlag(progressFlag.FlagId, progressFlag);
    }

    public void AddProgressFlag(ProgressFlagReference progressFlag)
    {
        if (!progressFlag.IsAssigned)
        {
            Debug.LogWarning("Tried to add an empty progress flag reference.");
            return;
        }

        AddProgressFlag(progressFlag.Id, progressFlag.Definition);
    }

    public void AddProgressFlag(string progressFlagId)
    {
        AddProgressFlag(progressFlagId, null);
    }

    private void AddProgressFlag(string progressFlagId, ProgressFlagDefinition definition)
    {
        if (string.IsNullOrWhiteSpace(progressFlagId))
        {
            Debug.LogWarning("Tried to add an empty progress flag.");
            return;
        }

        bool wasAdded = completedProgressFlagIds.Add(progressFlagId);

        if (!wasAdded)
        {
            return;
        }

        Debug.Log($"Progress flag added: {progressFlagId}");
        ProgressFlagAdded?.Invoke(progressFlagId);

        if (definition != null)
        {
            ProgressFlagDefinitionAdded?.Invoke(definition);
        }

        TryAutoAdvanceStage();
    }

    public void SetStage(GameStageDefinition newStage)
    {
        if (newStage == null)
        {
            Debug.LogWarning("Tried to set a null game stage.");
            return;
        }

        if (CurrentStage == newStage)
        {
            return;
        }

        if (!CanEnterStage(newStage))
        {
            Debug.LogWarning($"Cannot enter stage '{newStage.name}'. Requirements are not met.");
            return;
        }

        CurrentStage = newStage;

        Debug.Log($"Stage changed to: {newStage.DisplayName}");
        StageChanged?.Invoke(CurrentStage);
    }

    public bool CanEnterStage(GameStageDefinition stage)
    {
        if (stage == null)
        {
            return false;
        }

        foreach (ProgressFlagReference requiredFlag in stage.RequiredProgressFlags)
        {
            if (requiredFlag.IsAssigned && !HasProgressFlag(requiredFlag))
            {
                return false;
            }
        }

        return true;
    }

    private void TryAutoAdvanceStage()
    {
        if (allStages == null || allStages.Count == 0)
        {
            return;
        }

        int currentIndex = CurrentStage != null ? allStages.IndexOf(CurrentStage) : -1;

        for (int i = currentIndex + 1; i < allStages.Count; i++)
        {
            GameStageDefinition stage = allStages[i];

            if (stage == null)
            {
                continue;
            }

            if (CanEnterStage(stage))
            {
                SetStage(stage);
                return;
            }
        }
    }

    public IReadOnlyCollection<string> GetCompletedProgressFlagIds()
    {
        return completedProgressFlagIds;
    }

    public IReadOnlyCollection<string> GetCompletedProgressFlags()
    {
        return completedProgressFlagIds;
    }
}

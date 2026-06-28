using System;
using System.Collections.Generic;
using UnityEngine;

public class GameProgressManager : MonoBehaviour
{
    public static GameProgressManager Instance { get; private set; }

    [Header("Stages")]
    [SerializeField] private GameStageDefinition initialStage;
    [SerializeField] private List<GameStageDefinition> allStages = new();

    private readonly HashSet<string> completedProgressFlags = new();

    public GameStageDefinition CurrentStage { get; private set; }

    public event Action<GameStageDefinition> StageChanged;
    public event Action<string> ProgressFlagAdded;

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

    public bool HasProgressFlag(string progressFlag)
    {
        if (string.IsNullOrWhiteSpace(progressFlag))
        {
            return false;
        }

        return completedProgressFlags.Contains(progressFlag);
    }

    public void AddProgressFlag(string progressFlag)
    {
        if (string.IsNullOrWhiteSpace(progressFlag))
        {
            Debug.LogWarning("Tried to add an empty progress flag.");
            return;
        }

        bool wasAdded = completedProgressFlags.Add(progressFlag);

        if (!wasAdded)
        {
            return;
        }

        Debug.Log($"Progress flag added: {progressFlag}");
        ProgressFlagAdded?.Invoke(progressFlag);

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

        foreach (string requiredFlag in stage.RequiredProgressFlags)
        {
            if (!HasProgressFlag(requiredFlag))
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

    public IReadOnlyCollection<string> GetCompletedProgressFlags()
    {
        return completedProgressFlags;
    }
}

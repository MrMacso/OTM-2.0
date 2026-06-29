using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ObjectiveTrackerUI : MonoBehaviour
{
    [Serializable]
    private class ObjectiveStep
    {
        [TextArea]
        public string objectiveText;

        [Header("Availability")]
        public bool requireSpecificPeriod;
        public MuseumTimePeriod requiredPeriod = MuseumTimePeriod.Present;
        public string[] requiredFlags;
        public string[] blockedByFlags;

        [Header("Completion")]
        public string completionFlag;
    }

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI objectiveText;
    [SerializeField] private TextMeshProUGUI objectiveUpdatedText;
    [SerializeField] private GameObject objectivePanel;

    [Header("Objectives")]
    [SerializeField] private string prefix = "Objective: ";
    [SerializeField] private string completedText = "Search the museum for more evidence.";
    [SerializeField] private ObjectiveStep[] objectiveSteps;

    [Header("Notification")]
    [SerializeField] private float notificationSeconds = 2f;
    [SerializeField] private UnityEvent onObjectiveChanged;

    private GameProgressManager progressManager;
    private MuseumTimelineManager timelineManager;
    private bool isSubscribed;
    private ObjectiveStep currentStep;
    private float notificationTimer;

    private void OnEnable()
    {
        SubscribeToManagers();
        RefreshObjective(false);
    }

    private void Start()
    {
        SubscribeToManagers();
        RefreshObjective(false);
    }

    private void OnDisable()
    {
        UnsubscribeFromManagers();
    }

    private void SubscribeToManagers()
    {
        if (isSubscribed)
        {
            return;
        }

        progressManager = GameProgressManager.Instance;
        timelineManager = MuseumTimelineManager.Instance;

        if (progressManager != null)
        {
            progressManager.ProgressFlagAdded += HandleProgressFlagAdded;
            progressManager.StageChanged += HandleStageChanged;
        }

        if (timelineManager != null)
        {
            timelineManager.TimePeriodChanged += HandleTimePeriodChanged;
        }

        isSubscribed = progressManager != null || timelineManager != null;
    }

    private void UnsubscribeFromManagers()
    {
        if (progressManager != null)
        {
            progressManager.ProgressFlagAdded -= HandleProgressFlagAdded;
            progressManager.StageChanged -= HandleStageChanged;
        }

        if (timelineManager != null)
        {
            timelineManager.TimePeriodChanged -= HandleTimePeriodChanged;
        }

        isSubscribed = false;
    }

    private void Update()
    {
        UpdateNotification(Time.deltaTime);
    }

    public void RefreshObjective()
    {
        RefreshObjective(true);
    }

    private void RefreshObjective(bool showNotification)
    {
        ObjectiveStep nextStep = FindActiveStep();

        if (nextStep == currentStep && objectiveText != null && !string.IsNullOrWhiteSpace(objectiveText.text))
        {
            return;
        }

        currentStep = nextStep;
        string text = currentStep != null ? currentStep.objectiveText : completedText;

        if (objectivePanel != null)
        {
            objectivePanel.SetActive(!string.IsNullOrWhiteSpace(text));
        }

        if (objectiveText != null)
        {
            objectiveText.text = string.IsNullOrWhiteSpace(text) ? string.Empty : prefix + text;
        }

        if (showNotification)
        {
            ShowNotification(text);
            onObjectiveChanged?.Invoke();
        }
    }

    private ObjectiveStep FindActiveStep()
    {
        if (objectiveSteps == null)
        {
            return null;
        }

        foreach (ObjectiveStep step in objectiveSteps)
        {
            if (step == null)
            {
                continue;
            }

            if (IsCompleted(step))
            {
                continue;
            }

            if (IsAvailable(step))
            {
                return step;
            }
        }

        return null;
    }

    private bool IsAvailable(ObjectiveStep step)
    {
        if (step.requireSpecificPeriod)
        {
            if (timelineManager == null || timelineManager.CurrentPeriod != step.requiredPeriod)
            {
                return false;
            }
        }

        if (!HasAllFlags(step.requiredFlags))
        {
            return false;
        }

        if (HasAnyFlag(step.blockedByFlags))
        {
            return false;
        }

        return true;
    }

    private bool IsCompleted(ObjectiveStep step)
    {
        return !string.IsNullOrWhiteSpace(step.completionFlag) &&
               progressManager != null &&
               progressManager.HasProgressFlag(step.completionFlag);
    }

    private bool HasAllFlags(string[] flags)
    {
        if (flags == null || flags.Length == 0)
        {
            return true;
        }

        if (progressManager == null)
        {
            return false;
        }

        foreach (string flag in flags)
        {
            if (!string.IsNullOrWhiteSpace(flag) && !progressManager.HasProgressFlag(flag))
            {
                return false;
            }
        }

        return true;
    }

    private bool HasAnyFlag(string[] flags)
    {
        if (flags == null || flags.Length == 0 || progressManager == null)
        {
            return false;
        }

        foreach (string flag in flags)
        {
            if (!string.IsNullOrWhiteSpace(flag) && progressManager.HasProgressFlag(flag))
            {
                return true;
            }
        }

        return false;
    }

    private void ShowNotification(string text)
    {
        if (objectiveUpdatedText == null || string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        objectiveUpdatedText.gameObject.SetActive(true);
        objectiveUpdatedText.text = prefix + text;
        notificationTimer = notificationSeconds;
    }

    private void UpdateNotification(float deltaTime)
    {
        if (objectiveUpdatedText == null || notificationTimer <= 0f)
        {
            return;
        }

        notificationTimer -= deltaTime;

        if (notificationTimer <= 0f)
        {
            objectiveUpdatedText.gameObject.SetActive(false);
        }
    }

    private void HandleProgressFlagAdded(string flag)
    {
        RefreshObjective(true);
    }

    private void HandleStageChanged(GameStageDefinition stage)
    {
        RefreshObjective(true);
    }

    private void HandleTimePeriodChanged(MuseumTimePeriod period)
    {
        RefreshObjective(true);
    }
}

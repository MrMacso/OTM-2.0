using System;
using UnityEngine;
using UnityEngine.Events;

public class StageActivator : MonoBehaviour
{
    [Serializable]
    private class StageActivationRule
    {
        public GameStageDefinition stage;

        [Header("Objects")]
        public GameObject[] objectsToEnable;
        public GameObject[] objectsToDisable;

        [Header("Events")]
        public UnityEvent onStageEntered;
    }

    [SerializeField] private StageActivationRule[] activationRules;

    private GameProgressManager progressManager;

    private void OnEnable()
    {
        progressManager = GameProgressManager.Instance;

        if (progressManager == null)
        {
            Debug.LogWarning("StageActivator could not find GameProgressManager.");
            return;
        }

        progressManager.StageChanged += HandleStageChanged;

        if (progressManager.CurrentStage != null)
        {
            HandleStageChanged(progressManager.CurrentStage);
        }
    }

    private void OnDisable()
    {
        if (progressManager != null)
        {
            progressManager.StageChanged -= HandleStageChanged;
        }
    }

    private void HandleStageChanged(GameStageDefinition currentStage)
    {
        foreach (StageActivationRule rule in activationRules)
        {
            if (rule == null || rule.stage == null)
            {
                continue;
            }

            bool isCurrentStage = rule.stage == currentStage;

            SetObjectsActive(rule.objectsToEnable, isCurrentStage);
            SetObjectsActive(rule.objectsToDisable, !isCurrentStage);

            if (isCurrentStage)
            {
                rule.onStageEntered?.Invoke();
            }
        }
    }

    private void SetObjectsActive(GameObject[] objects, bool active)
    {
        if (objects == null)
        {
            return;
        }

        foreach (GameObject target in objects)
        {
            if (target != null)
            {
                target.SetActive(active);
            }
        }
    }
}
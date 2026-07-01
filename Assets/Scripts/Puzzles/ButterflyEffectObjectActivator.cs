using System;
using UnityEngine;

public class ButterflyEffectObjectActivator : MonoBehaviour
{
    [Serializable]
    private class ButterflyRule
    {
        public string label;
        public bool requireSpecificPeriod;
        public MuseumTimePeriod period = MuseumTimePeriod.Present;
        public ProgressFlagReference[] requiredFlags;
        public ProgressFlagReference[] blockedByFlags;
        public GameObject[] objectsToEnable;
        public GameObject[] objectsToDisable;
    }

    [SerializeField] private ButterflyRule[] rules;

    private MuseumTimelineManager timelineManager;
    private GameProgressManager progressManager;

    private void OnEnable()
    {
        timelineManager = MuseumTimelineManager.Instance;
        progressManager = GameProgressManager.Instance;

        if (timelineManager != null)
        {
            timelineManager.TimePeriodChanged += HandleTimePeriodChanged;
        }

        if (progressManager != null)
        {
            progressManager.ProgressFlagAdded += HandleProgressFlagAdded;
        }

        ApplyRules();
    }

    private void OnDisable()
    {
        if (timelineManager != null)
        {
            timelineManager.TimePeriodChanged -= HandleTimePeriodChanged;
        }

        if (progressManager != null)
        {
            progressManager.ProgressFlagAdded -= HandleProgressFlagAdded;
        }
    }

    private void HandleTimePeriodChanged(MuseumTimePeriod period)
    {
        ApplyRules();
    }

    private void HandleProgressFlagAdded(string flag)
    {
        ApplyRules();
    }

    public void ApplyRules()
    {
        if (rules == null)
        {
            return;
        }

        foreach (ButterflyRule rule in rules)
        {
            if (rule == null)
            {
                continue;
            }

            bool matches = RuleMatches(rule);
            SetObjectsActive(rule.objectsToEnable, matches);
            SetObjectsActive(rule.objectsToDisable, !matches);
        }
    }

    private bool RuleMatches(ButterflyRule rule)
    {
        if (rule.requireSpecificPeriod)
        {
            if (timelineManager == null || timelineManager.CurrentPeriod != rule.period)
            {
                return false;
            }
        }

        if (!HasAllFlags(rule.requiredFlags))
        {
            return false;
        }

        if (HasAnyFlag(rule.blockedByFlags))
        {
            return false;
        }

        return true;
    }

    private bool HasAllFlags(ProgressFlagReference[] flags)
    {
        if (flags == null || flags.Length == 0)
        {
            return true;
        }

        if (progressManager == null)
        {
            return false;
        }

        foreach (ProgressFlagReference flag in flags)
        {
            if (flag.IsAssigned && !progressManager.HasProgressFlag(flag))
            {
                return false;
            }
        }

        return true;
    }

    private bool HasAnyFlag(ProgressFlagReference[] flags)
    {
        if (flags == null || flags.Length == 0 || progressManager == null)
        {
            return false;
        }

        foreach (ProgressFlagReference flag in flags)
        {
            if (flag.IsAssigned && progressManager.HasProgressFlag(flag))
            {
                return true;
            }
        }

        return false;
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

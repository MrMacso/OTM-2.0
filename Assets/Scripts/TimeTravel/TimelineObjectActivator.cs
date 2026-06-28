using System;
using UnityEngine;

public class TimelineObjectActivator : MonoBehaviour
{
    [Serializable]
    private class TimelineObjectGroup
    {
        public MuseumTimePeriod timePeriod;
        public GameObject[] objectsToEnable;
    }

    [SerializeField] private TimelineObjectGroup[] timelineGroups;

    private MuseumTimelineManager timelineManager;

    private void OnEnable()
    {
        timelineManager = MuseumTimelineManager.Instance;

        if (timelineManager == null)
        {
            Debug.LogWarning("TimelineObjectActivator could not find MuseumTimelineManager.");
            return;
        }

        timelineManager.TimePeriodChanged += HandleTimePeriodChanged;
        HandleTimePeriodChanged(timelineManager.CurrentPeriod);
    }

    private void OnDisable()
    {
        if (timelineManager != null)
        {
            timelineManager.TimePeriodChanged -= HandleTimePeriodChanged;
        }
    }

    private void HandleTimePeriodChanged(MuseumTimePeriod currentPeriod)
    {
        foreach (TimelineObjectGroup group in timelineGroups)
        {
            if (group == null)
            {
                continue;
            }

            bool shouldBeActive = group.timePeriod == currentPeriod;
            SetObjectsActive(group.objectsToEnable, shouldBeActive);
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
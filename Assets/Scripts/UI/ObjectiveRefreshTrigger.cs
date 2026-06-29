using UnityEngine;

public class ObjectiveRefreshTrigger : MonoBehaviour
{
    [SerializeField] private ObjectiveTrackerUI objectiveTracker;

    public void RefreshObjective()
    {
        if (objectiveTracker == null)
        {
            objectiveTracker = FindFirstObjectByType<ObjectiveTrackerUI>();
        }

        if (objectiveTracker == null)
        {
            Debug.LogWarning("No ObjectiveTrackerUI found in scene.");
            return;
        }

        objectiveTracker.RefreshObjective();
    }
}

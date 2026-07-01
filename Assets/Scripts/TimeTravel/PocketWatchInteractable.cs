using UnityEngine;
using UnityEngine.Events;

public class PocketWatchInteractable : MonoBehaviour, IInteractable
{
    [Header("Interaction")]
    [SerializeField] private string unboundPrompt = "Press E to wind the pocket watch";
    [SerializeField] private string boundPrompt = "Press E to return to the present";
    [SerializeField] private bool disableObjectAfterFirstUse = false;

    [Header("Travel")]
    [SerializeField] private MuseumTimelineManager timelineManager;
    [SerializeField] private WatchPulseSystem watchPulseSystem;
    [SerializeField] private MuseumTimePeriod firstTravelPeriod = MuseumTimePeriod.Past;

    [Header("Progress")]
    [SerializeField] private ProgressFlagReference pocketWatchCollectedFlag = new ProgressFlagReference(ProgressFlags.PocketWatchCollected);

    [Header("Events")]
    [SerializeField] private UnityEvent onFirstWatchBound;
    [SerializeField] private UnityEvent onWatchUsed;

    private bool hasBoundWatch;

    public string InteractionPrompt
    {
        get
        {
            if (!hasBoundWatch || MuseumTimelineManager.Instance == null)
            {
                return unboundPrompt;
            }

            return MuseumTimelineManager.Instance.CurrentPeriod == MuseumTimePeriod.Present
                ? unboundPrompt
                : boundPrompt;
        }
    }

    public void Interact(GameObject interactor)
    {
        if (timelineManager == null)
        {
            timelineManager = MuseumTimelineManager.Instance;
        }

        if (watchPulseSystem == null)
        {
            watchPulseSystem = WatchPulseSystem.Instance;
        }

        if (timelineManager == null)
        {
            Debug.LogWarning("No MuseumTimelineManager found in scene.");
            return;
        }

        if (!hasBoundWatch)
        {
            BindWatch();
        }

        if (timelineManager.CurrentPeriod == MuseumTimePeriod.Present)
        {
            watchPulseSystem?.ResetPastTimer();
            timelineManager.TravelTo(firstTravelPeriod);
        }
        else
        {
            timelineManager.ReturnToPresent();
        }

        onWatchUsed?.Invoke();

        if (disableObjectAfterFirstUse)
        {
            gameObject.SetActive(false);
        }
    }

    public void BindWatch()
    {
        if (hasBoundWatch)
        {
            return;
        }

        hasBoundWatch = true;

        if (pocketWatchCollectedFlag.IsAssigned)
        {
            GameProgressManager.Instance?.AddProgressFlag(pocketWatchCollectedFlag);
        }

        onFirstWatchBound?.Invoke();
    }
}

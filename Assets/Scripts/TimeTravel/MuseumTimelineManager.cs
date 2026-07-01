using System;
using UnityEngine;
using UnityEngine.Events;

public class MuseumTimelineManager : MonoBehaviour
{
    public static MuseumTimelineManager Instance { get; private set; }

    [SerializeField] private MuseumTimePeriod startingPeriod = MuseumTimePeriod.Present;
    [SerializeField] private bool addProgressFlagOnFirstTravel = true;
    [SerializeField] private ProgressFlagReference firstTimeTravelCompletedFlag = new ProgressFlagReference(ProgressFlags.FirstTimeTravelCompleted);

    [Header("Events")]
    [SerializeField] private UnityEvent onTravelToPresent;
    [SerializeField] private UnityEvent onTravelToPast;
    [SerializeField] private UnityEvent onFirstTimeTravelCompleted;

    private bool hasCompletedFirstTimeTravel;

    public MuseumTimePeriod CurrentPeriod { get; private set; }

    public event Action<MuseumTimePeriod> TimePeriodChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Duplicate MuseumTimelineManager found. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        CurrentPeriod = startingPeriod;
    }

    private void Start()
    {
        TimePeriodChanged?.Invoke(CurrentPeriod);
    }

    public void TravelTo(MuseumTimePeriod newPeriod)
    {
        SetTimePeriod(newPeriod);
    }

    public void TravelToPast()
    {
        SetTimePeriod(MuseumTimePeriod.Past);
    }

    public void ReturnToPresent()
    {
        SetTimePeriod(MuseumTimePeriod.Present);
    }

    public void SetTimePeriod(MuseumTimePeriod newPeriod)
    {
        if (CurrentPeriod == newPeriod)
        {
            return;
        }

        MuseumTimePeriod previousPeriod = CurrentPeriod;
        CurrentPeriod = newPeriod;

        Debug.Log($"Museum time period changed to: {CurrentPeriod}");
        TimePeriodChanged?.Invoke(CurrentPeriod);
        InvokePeriodEvents(CurrentPeriod);

        if (addProgressFlagOnFirstTravel &&
            !hasCompletedFirstTimeTravel &&
            previousPeriod == MuseumTimePeriod.Present &&
            CurrentPeriod != MuseumTimePeriod.Present)
        {
            hasCompletedFirstTimeTravel = true;

            if (firstTimeTravelCompletedFlag.IsAssigned)
            {
                GameProgressManager.Instance?.AddProgressFlag(firstTimeTravelCompletedFlag);
            }

            onFirstTimeTravelCompleted?.Invoke();
        }
    }

    public void TogglePastAndPresent()
    {
        if (CurrentPeriod == MuseumTimePeriod.Present)
        {
            TravelToPast();
        }
        else
        {
            ReturnToPresent();
        }
    }

    private void InvokePeriodEvents(MuseumTimePeriod period)
    {
        switch (period)
        {
            case MuseumTimePeriod.Present:
                onTravelToPresent?.Invoke();
                break;
            case MuseumTimePeriod.Past:
                onTravelToPast?.Invoke();
                break;
        }
    }
}

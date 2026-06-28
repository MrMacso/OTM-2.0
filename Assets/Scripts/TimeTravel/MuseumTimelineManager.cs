using System;
using UnityEngine;

public class MuseumTimelineManager : MonoBehaviour
{
    public static MuseumTimelineManager Instance { get; private set; }

    [SerializeField] private MuseumTimePeriod startingPeriod = MuseumTimePeriod.Present;
    [SerializeField] private bool addProgressFlagOnFirstTravel = true;

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

        if (addProgressFlagOnFirstTravel &&
            !hasCompletedFirstTimeTravel &&
            previousPeriod == MuseumTimePeriod.Present &&
            CurrentPeriod != MuseumTimePeriod.Present)
        {
            hasCompletedFirstTimeTravel = true;

            if (GameProgressManager.Instance != null)
            {
                GameProgressManager.Instance.AddProgressFlag(ProgressFlags.FirstTimeTravelCompleted);
            }
        }
    }

    public void TogglePastAndPresent()
    {
        if (CurrentPeriod == MuseumTimePeriod.Present)
        {
            SetTimePeriod(MuseumTimePeriod.Past);
        }
        else
        {
            SetTimePeriod(MuseumTimePeriod.Present);
        }
    }
}

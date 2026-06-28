using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WatchPulseSystem : MonoBehaviour
{
    public static WatchPulseSystem Instance { get; private set; }

    [Header("References")]
    [SerializeField] private MuseumTimelineManager timelineManager;
    [SerializeField] private PlayerControlStateController playerControl;

    [Header("Pulse")]
    [SerializeField] private float restingPulse = 72f;
    [SerializeField] private float calmPastPulse = 92f;
    [SerializeField] private float criticalPulse = 165f;
    [SerializeField] private float presentRecoveryPerSecond = 12f;
    [SerializeField] private float calmPastIncreasePerSecond = 3f;
    [SerializeField] private float panicIncreasePerSecond = 28f;
    [SerializeField] private float breathingRecoveryPerSecond = 18f;

    [Header("Watch Timer")]
    [SerializeField] private float maxPastSeconds = 90f;
    [SerializeField] private float calmDrainPerSecond = 1f;
    [SerializeField] private float panicDrainPerSecond = 6f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI pulseText;
    [SerializeField] private TextMeshProUGUI watchText;
    [SerializeField] private Image pulseFillImage;
    [SerializeField] private Image watchFillImage;

    [Header("Events")]
    [SerializeField] private UnityEvent onPanicStarted;
    [SerializeField] private UnityEvent onPanicEnded;
    [SerializeField] private UnityEvent onCriticalPulse;
    [SerializeField] private UnityEvent onForcedReturn;

    private float currentPulse;
    private float remainingPastSeconds;
    private bool isPanicking;
    private bool isBreathing;
    private bool isInPast;

    public float CurrentPulse => currentPulse;
    public float RemainingPastSeconds => remainingPastSeconds;
    public float PulseNormalized => Mathf.InverseLerp(restingPulse, criticalPulse, currentPulse);
    public float WatchNormalized => maxPastSeconds <= 0f ? 0f : Mathf.Clamp01(remainingPastSeconds / maxPastSeconds);
    public bool IsPanicking => isPanicking;
    public bool IsBreathing => isBreathing;

    public event Action<float> PulseChanged;
    public event Action<float> WatchTimeChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Duplicate WatchPulseSystem found. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        currentPulse = restingPulse;
        remainingPastSeconds = maxPastSeconds;
    }

    private void OnEnable()
    {
        if (timelineManager == null)
        {
            timelineManager = MuseumTimelineManager.Instance;
        }

        if (timelineManager != null)
        {
            timelineManager.TimePeriodChanged += HandleTimePeriodChanged;
            HandleTimePeriodChanged(timelineManager.CurrentPeriod);
        }
    }

    private void Start()
    {
        if (timelineManager == null)
        {
            timelineManager = MuseumTimelineManager.Instance;
        }

        if (timelineManager != null)
        {
            timelineManager.TimePeriodChanged -= HandleTimePeriodChanged;
            timelineManager.TimePeriodChanged += HandleTimePeriodChanged;
            HandleTimePeriodChanged(timelineManager.CurrentPeriod);
        }
    }

    private void OnDisable()
    {
        if (timelineManager != null)
        {
            timelineManager.TimePeriodChanged -= HandleTimePeriodChanged;
        }
    }

    private void Update()
    {
        UpdatePulse(Time.deltaTime);
        UpdateWatch(Time.deltaTime);
        UpdateUI();
    }

    public void StartPanic()
    {
        if (isPanicking)
        {
            return;
        }

        isPanicking = true;
        onPanicStarted?.Invoke();
    }

    public void EndPanic()
    {
        if (!isPanicking)
        {
            return;
        }

        isPanicking = false;
        onPanicEnded?.Invoke();
    }

    public void SetBreathing(bool breathing)
    {
        isBreathing = breathing;
    }

    public void AddPulse(float amount)
    {
        currentPulse = Mathf.Clamp(currentPulse + amount, restingPulse, criticalPulse);
        PulseChanged?.Invoke(currentPulse);

        if (currentPulse >= criticalPulse)
        {
            TriggerCriticalPulse();
        }
    }

    public void Stabilize(float amount)
    {
        currentPulse = Mathf.Clamp(currentPulse - amount, restingPulse, criticalPulse);
        PulseChanged?.Invoke(currentPulse);
    }

    public void ResetPastTimer()
    {
        remainingPastSeconds = maxPastSeconds;
        WatchTimeChanged?.Invoke(remainingPastSeconds);
    }

    private void HandleTimePeriodChanged(MuseumTimePeriod period)
    {
        isInPast = period != MuseumTimePeriod.Present;

        if (isInPast)
        {
            ResetPastTimer();
        }
        else
        {
            EndPanic();
            SetBreathing(false);
        }
    }

    private void UpdatePulse(float deltaTime)
    {
        float targetPulse = isInPast ? calmPastPulse : restingPulse;
        float rate = isInPast ? calmPastIncreasePerSecond : presentRecoveryPerSecond;

        if (isPanicking)
        {
            targetPulse = criticalPulse;
            rate = panicIncreasePerSecond;
        }
        else if (isBreathing)
        {
            targetPulse = restingPulse;
            rate = breathingRecoveryPerSecond;
        }

        currentPulse = Mathf.MoveTowards(currentPulse, targetPulse, rate * deltaTime);
        PulseChanged?.Invoke(currentPulse);

        if (currentPulse >= criticalPulse)
        {
            TriggerCriticalPulse();
        }
    }

    private void UpdateWatch(float deltaTime)
    {
        if (!isInPast)
        {
            return;
        }

        float drainRate = isPanicking ? panicDrainPerSecond : calmDrainPerSecond;
        remainingPastSeconds = Mathf.Max(0f, remainingPastSeconds - drainRate * deltaTime);
        WatchTimeChanged?.Invoke(remainingPastSeconds);

        if (remainingPastSeconds <= 0f)
        {
            TriggerCriticalPulse();
        }
    }

    private void TriggerCriticalPulse()
    {
        if (!isInPast)
        {
            return;
        }

        onCriticalPulse?.Invoke();
        ForceReturnToPresent();
    }

    private void ForceReturnToPresent()
    {
        EndPanic();
        SetBreathing(false);
        currentPulse = Mathf.Min(currentPulse, criticalPulse);

        if (playerControl != null)
        {
            playerControl.SetTimeTravelMode();
        }

        if (timelineManager != null)
        {
            timelineManager.ReturnToPresent();
        }

        if (playerControl != null)
        {
            playerControl.SetGameplayMode();
        }

        onForcedReturn?.Invoke();
    }

    private void UpdateUI()
    {
        if (pulseText != null)
        {
            pulseText.text = $"Pulse {Mathf.RoundToInt(currentPulse)}";
        }

        if (watchText != null)
        {
            watchText.text = isInPast ? $"Watch {Mathf.CeilToInt(remainingPastSeconds)}s" : "Watch stable";
        }

        if (pulseFillImage != null)
        {
            pulseFillImage.fillAmount = PulseNormalized;
        }

        if (watchFillImage != null)
        {
            watchFillImage.fillAmount = WatchNormalized;
        }
    }
}

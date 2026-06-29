using UnityEngine;
using UnityEngine.Events;

public class BreathingZone : MonoBehaviour
{
    [Header("Timeline")]
    [SerializeField] private bool requireSpecificPeriod = true;
    [SerializeField] private MuseumTimePeriod requiredPeriod = MuseumTimePeriod.Past;

    [Header("Recovery Requirements")]
    [SerializeField] private bool requireCrouching = true;
    [SerializeField] private bool requireStillness = true;
    [SerializeField] private float secondsRequiredToRecover = 2.5f;

    [Header("Feedback")]
    [SerializeField] private string enterFeedback = "Stay low. Breathe slowly.";
    [SerializeField] private string requirementFeedback = "Crouch and stay still to steady your breathing.";
    [SerializeField] private string recoveredFeedback = "Your breathing steadies.";
    [SerializeField] private float requirementHintCooldown = 2f;

    [Header("Progress")]
    [SerializeField] private string recoveredFlag = ProgressFlags.FirstBreathingRecovered;
    [SerializeField] private bool addRecoveredFlagOnlyOnce = true;

    [Header("Events")]
    [SerializeField] private UnityEvent onEntered;
    [SerializeField] private UnityEvent onRecoveryStarted;
    [SerializeField] private UnityEvent onRecoveryInterrupted;
    [SerializeField] private UnityEvent onRecovered;
    [SerializeField] private UnityEvent onExited;

    private InputSystemFPSController playerController;
    private bool playerInside;
    private bool isRecovering;
    private bool hasRecovered;
    private float recoveryTimer;
    private float nextRequirementHintTime;

    private void Update()
    {
        if (!playerInside)
        {
            return;
        }

        UpdateRecovery(Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerController = other.GetComponent<InputSystemFPSController>();
        playerInside = true;
        recoveryTimer = 0f;

        FeedbackMessageUI.Instance?.ShowMessage(enterFeedback);
        onEntered?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        StopRecovery(true);
        playerController = null;
        playerInside = false;
        recoveryTimer = 0f;
        onExited?.Invoke();
    }

    private void UpdateRecovery(float deltaTime)
    {
        if (hasRecovered && addRecoveredFlagOnlyOnce)
        {
            return;
        }

        if (!CanRecover())
        {
            if (isRecovering)
            {
                StopRecovery(true);
            }

            return;
        }

        if (!isRecovering)
        {
            StartRecovery();
        }

        recoveryTimer += deltaTime;

        if (recoveryTimer >= secondsRequiredToRecover)
        {
            CompleteRecovery();
        }
    }

    private void StartRecovery()
    {
        isRecovering = true;
        WatchPulseSystem.Instance?.SetBreathing(true);
        onRecoveryStarted?.Invoke();
    }

    private void StopRecovery(bool interrupted)
    {
        if (!isRecovering)
        {
            return;
        }

        isRecovering = false;
        WatchPulseSystem.Instance?.SetBreathing(false);

        if (interrupted)
        {
            recoveryTimer = 0f;
            onRecoveryInterrupted?.Invoke();
        }
    }

    private void CompleteRecovery()
    {
        if (hasRecovered && addRecoveredFlagOnlyOnce)
        {
            return;
        }

        hasRecovered = true;
        WatchPulseSystem.Instance?.EndPanic();
        WatchPulseSystem.Instance?.SetBreathing(false);
        FeedbackMessageUI.Instance?.ShowMessage(recoveredFeedback);

        if (!string.IsNullOrWhiteSpace(recoveredFlag))
        {
            GameProgressManager.Instance?.AddProgressFlag(recoveredFlag);
        }

        StopRecovery(false);
        onRecovered?.Invoke();
    }

    private bool CanRecover()
    {
        if (requireSpecificPeriod)
        {
            if (MuseumTimelineManager.Instance == null ||
                MuseumTimelineManager.Instance.CurrentPeriod != requiredPeriod)
            {
                return false;
            }
        }

        if (playerController == null)
        {
            return false;
        }

        if (requireCrouching && !playerController.IsCrouching)
        {
            ShowRequirementHint();
            return false;
        }

        if (requireStillness && playerController.IsMoving)
        {
            ShowRequirementHint();
            return false;
        }

        return true;
    }

    private void ShowRequirementHint()
    {
        if (isRecovering)
        {
            return;
        }

        if (Time.time < nextRequirementHintTime)
        {
            return;
        }

        nextRequirementHintTime = Time.time + requirementHintCooldown;
        FeedbackMessageUI.Instance?.ShowWarning(requirementFeedback);
    }
}

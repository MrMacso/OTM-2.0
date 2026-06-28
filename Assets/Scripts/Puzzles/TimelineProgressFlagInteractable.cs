using UnityEngine;
using UnityEngine.Events;

public class TimelineProgressFlagInteractable : MonoBehaviour, IInteractable
{
    [Header("Interaction")]
    [SerializeField] private string prompt = "Press E to interact";
    [SerializeField] private string wrongPeriodPrompt = "This does not belong to this time.";

    [Header("Timeline")]
    [SerializeField] private MuseumTimePeriod requiredPeriod = MuseumTimePeriod.Past;
    [SerializeField] private bool requireExactPeriod = true;

    [Header("Progress")]
    [SerializeField] private string progressFlagToAdd = ProgressFlags.CassetteKeyStolenPast;
    [SerializeField] private bool disableAfterInteraction = true;

    [Header("Events")]
    [SerializeField] private UnityEvent onInteracted;
    [SerializeField] private UnityEvent onWrongPeriod;

    private bool hasBeenUsed;

    public string InteractionPrompt => CanInteractInCurrentPeriod() ? prompt : wrongPeriodPrompt;

    public void Interact(GameObject interactor)
    {
        if (hasBeenUsed)
        {
            return;
        }

        if (!CanInteractInCurrentPeriod())
        {
            onWrongPeriod?.Invoke();
            return;
        }

        if (string.IsNullOrWhiteSpace(progressFlagToAdd))
        {
            Debug.LogWarning($"{nameof(TimelineProgressFlagInteractable)} on {name} has no progress flag assigned.");
            return;
        }

        if (GameProgressManager.Instance == null)
        {
            Debug.LogWarning("No GameProgressManager found in scene.");
            return;
        }

        GameProgressManager.Instance.AddProgressFlag(progressFlagToAdd);
        onInteracted?.Invoke();
        hasBeenUsed = true;

        if (disableAfterInteraction)
        {
            gameObject.SetActive(false);
        }
    }

    private bool CanInteractInCurrentPeriod()
    {
        if (!requireExactPeriod)
        {
            return true;
        }

        return MuseumTimelineManager.Instance != null &&
               MuseumTimelineManager.Instance.CurrentPeriod == requiredPeriod;
    }
}

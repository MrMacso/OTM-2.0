using UnityEngine;
using UnityEngine.Events;

public class LockedEvidenceBoxInteractable : MonoBehaviour, IInteractable
{
    [Header("Prompts")]
    [SerializeField] private string lockedPrompt = "Locked. The key is missing.";
    [SerializeField] private string openPrompt = "Press E to open the cassette box";
    [SerializeField] private string collectEvidencePrompt = "Press E to take the evidence";
    [SerializeField] private string completedPrompt = "The cassette box is empty.";

    [Header("Feedback")]
    [SerializeField] private string lockedFeedback = "The lock is old, but the key is missing.";
    [SerializeField] private string openedFeedback = "The cassette box clicks open.";
    [SerializeField] private string evidenceFeedback = "A name, a date, and a room number. This changes everything.";
    [SerializeField] private string wrongPeriodFeedback = "This evidence is not here in this time.";

    [Header("Requirements")]
    [SerializeField] private MuseumTimePeriod requiredPeriod = MuseumTimePeriod.Present;
    [SerializeField] private ProgressFlagReference requiredFlagToOpen = new ProgressFlagReference(ProgressFlags.CassetteKeyStolenPast);

    [Header("Progress")]
    [SerializeField] private ProgressFlagReference openedFlag = new ProgressFlagReference(ProgressFlags.CassetteBoxOpenedPresent);
    [SerializeField] private ProgressFlagReference evidenceFoundFlag = new ProgressFlagReference(ProgressFlags.CassetteEvidenceFound);
    [SerializeField] private ProgressFlagReference completedPuzzleFlag = new ProgressFlagReference(ProgressFlags.BasementPuzzleSolved);
    [SerializeField] private bool completePuzzleWhenEvidenceTaken = true;

    [Header("Object States")]
    [SerializeField] private GameObject closedVisual;
    [SerializeField] private GameObject openVisual;
    [SerializeField] private GameObject evidenceObject;

    [Header("Events")]
    [SerializeField] private UnityEvent onLockedTried;
    [SerializeField] private UnityEvent onOpened;
    [SerializeField] private UnityEvent onEvidenceCollected;
    [SerializeField] private UnityEvent onWrongPeriod;

    private bool isOpen;
    private bool evidenceCollected;

    public string InteractionPrompt
    {
        get
        {
            if (!IsInRequiredPeriod())
            {
                return wrongPeriodFeedback;
            }

            if (evidenceCollected)
            {
                return completedPrompt;
            }

            if (isOpen)
            {
                return collectEvidencePrompt;
            }

            return HasRequiredFlag() ? openPrompt : lockedPrompt;
        }
    }

    private void Start()
    {
        RestoreProgressState();
        ApplyVisualState();
    }

    public void Interact(GameObject interactor)
    {
        if (!IsInRequiredPeriod())
        {
            FeedbackMessageUI.Instance?.ShowWarning(wrongPeriodFeedback);
            onWrongPeriod?.Invoke();
            return;
        }

        if (evidenceCollected)
        {
            return;
        }

        if (!isOpen)
        {
            TryOpen();
            return;
        }

        CollectEvidence();
    }

    public void TryOpen()
    {
        if (!HasRequiredFlag())
        {
            FeedbackMessageUI.Instance?.ShowWarning(lockedFeedback);
            onLockedTried?.Invoke();
            return;
        }

        isOpen = true;

        if (openedFlag.IsAssigned)
        {
            GameProgressManager.Instance?.AddProgressFlag(openedFlag);
        }

        FeedbackMessageUI.Instance?.ShowMessage(openedFeedback);
        ApplyVisualState();
        onOpened?.Invoke();
    }

    public void CollectEvidence()
    {
        if (!isOpen || evidenceCollected)
        {
            return;
        }

        evidenceCollected = true;

        if (evidenceFoundFlag.IsAssigned)
        {
            GameProgressManager.Instance?.AddProgressFlag(evidenceFoundFlag);
        }

        if (completePuzzleWhenEvidenceTaken && completedPuzzleFlag.IsAssigned)
        {
            GameProgressManager.Instance?.AddProgressFlag(completedPuzzleFlag);
        }

        FeedbackMessageUI.Instance?.ShowDiscovery(evidenceFeedback);
        ApplyVisualState();
        onEvidenceCollected?.Invoke();
    }

    private void RestoreProgressState()
    {
        if (GameProgressManager.Instance == null)
        {
            return;
        }

        isOpen = openedFlag.IsAssigned && GameProgressManager.Instance.HasProgressFlag(openedFlag);
        evidenceCollected = evidenceFoundFlag.IsAssigned && GameProgressManager.Instance.HasProgressFlag(evidenceFoundFlag);
    }

    private bool HasRequiredFlag()
    {
        return !requiredFlagToOpen.IsAssigned ||
               (GameProgressManager.Instance != null && GameProgressManager.Instance.HasProgressFlag(requiredFlagToOpen));
    }

    private bool IsInRequiredPeriod()
    {
        return MuseumTimelineManager.Instance == null ||
               MuseumTimelineManager.Instance.CurrentPeriod == requiredPeriod;
    }

    private void ApplyVisualState()
    {
        if (closedVisual != null)
        {
            closedVisual.SetActive(!isOpen);
        }

        if (openVisual != null)
        {
            openVisual.SetActive(isOpen);
        }

        if (evidenceObject != null)
        {
            evidenceObject.SetActive(isOpen && !evidenceCollected);
        }
    }
}

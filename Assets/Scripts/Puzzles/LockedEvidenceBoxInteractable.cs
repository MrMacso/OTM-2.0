using UnityEngine;
using UnityEngine.Events;

public class LockedEvidenceBoxInteractable : MonoBehaviour, IInteractable
{
    [Header("Prompts")]
    [SerializeField] private string lockedPrompt = "Locked. The key is missing.";
    [SerializeField] private string openPrompt = "Press E to open the cassette box";
    [SerializeField] private string collectEvidencePrompt = "Press E to take the evidence";
    [SerializeField] private string completedPrompt = "The cassette box is empty.";

    [Header("Requirements")]
    [SerializeField] private MuseumTimePeriod requiredPeriod = MuseumTimePeriod.Present;
    [SerializeField] private string requiredFlagToOpen = ProgressFlags.CassetteKeyStolenPast;

    [Header("Progress")]
    [SerializeField] private string openedFlag = ProgressFlags.CassetteBoxOpenedPresent;
    [SerializeField] private string evidenceFoundFlag = ProgressFlags.CassetteEvidenceFound;
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
                return "This evidence is not here in this time.";
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
            onLockedTried?.Invoke();
            return;
        }

        isOpen = true;
        GameProgressManager.Instance?.AddProgressFlag(openedFlag);
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
        GameProgressManager.Instance?.AddProgressFlag(evidenceFoundFlag);

        if (completePuzzleWhenEvidenceTaken)
        {
            GameProgressManager.Instance?.AddProgressFlag(ProgressFlags.BasementPuzzleSolved);
        }

        ApplyVisualState();
        onEvidenceCollected?.Invoke();
    }

    private void RestoreProgressState()
    {
        if (GameProgressManager.Instance == null)
        {
            return;
        }

        isOpen = !string.IsNullOrWhiteSpace(openedFlag) &&
                 GameProgressManager.Instance.HasProgressFlag(openedFlag);
        evidenceCollected = !string.IsNullOrWhiteSpace(evidenceFoundFlag) &&
                            GameProgressManager.Instance.HasProgressFlag(evidenceFoundFlag);
    }

    private bool HasRequiredFlag()
    {
        return string.IsNullOrWhiteSpace(requiredFlagToOpen) ||
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

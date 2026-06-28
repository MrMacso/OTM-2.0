using UnityEngine;
using UnityEngine.Events;

public class InspectableObject : MonoBehaviour, IInteractable
{
    [Header("Inspection")]
    [SerializeField] private GameObject inspectionView;
    [SerializeField] private string prompt = "Press E to inspect";
    [SerializeField] private string progressFlagOnInspect;

    [Header("Events")]
    [SerializeField] private UnityEvent onInspectionStarted;
    [SerializeField] private UnityEvent onInspectionEnded;

    private PlayerControlStateController playerControl;
    private bool isInspecting;

    public string InteractionPrompt => prompt;

    public void Interact(GameObject interactor)
    {
        if (isInspecting)
        {
            return;
        }

        playerControl = interactor.GetComponent<PlayerControlStateController>();

        if (playerControl == null)
        {
            Debug.LogWarning("Interactor does not have PlayerControlStateController.");
            return;
        }

        StartInspection();
    }

    public void StartInspection()
    {
        isInspecting = true;

        if (inspectionView != null)
        {
            inspectionView.SetActive(true);
        }

        playerControl.SetInspectingMode();

        if (!string.IsNullOrWhiteSpace(progressFlagOnInspect) &&
            GameProgressManager.Instance != null)
        {
            GameProgressManager.Instance.AddProgressFlag(progressFlagOnInspect);
        }

        onInspectionStarted?.Invoke();
    }

    public void EndInspection()
    {
        if (!isInspecting)
        {
            return;
        }

        isInspecting = false;

        if (inspectionView != null)
        {
            inspectionView.SetActive(false);
        }

        if (playerControl != null)
        {
            playerControl.SetGameplayMode();
        }

        onInspectionEnded?.Invoke();
    }
}

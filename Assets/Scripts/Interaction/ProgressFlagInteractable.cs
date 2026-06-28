using UnityEngine;
using UnityEngine.Events;

public class ProgressFlagInteractable : MonoBehaviour, IInteractable
{
    [Header("Interaction")]
    [SerializeField] private string prompt = "Press E to interact";

    [Header("Progress")]
    [SerializeField] private string progressFlagToAdd;
    [SerializeField] private bool disableAfterInteraction = true;

    [Header("Events")]
    [SerializeField] private UnityEvent onInteracted;

    private bool hasBeenUsed;

    public string InteractionPrompt => prompt;

    public void Interact(GameObject interactor)
    {
        if (hasBeenUsed)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(progressFlagToAdd))
        {
            Debug.LogWarning($"{nameof(ProgressFlagInteractable)} on {name} has no progress flag assigned.");
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
}

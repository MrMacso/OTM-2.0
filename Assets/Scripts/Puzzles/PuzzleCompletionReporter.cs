using UnityEngine;
using UnityEngine.Events;

public class PuzzleCompletionReporter : MonoBehaviour
{
    [SerializeField] private string completionProgressFlag = ProgressFlags.BasementPuzzleSolved;
    [SerializeField] private bool reportOnlyOnce = true;
    [SerializeField] private UnityEvent onPuzzleCompleted;

    private bool hasReported;

    public void ReportCompleted()
    {
        if (hasReported && reportOnlyOnce)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(completionProgressFlag))
        {
            Debug.LogWarning($"{nameof(PuzzleCompletionReporter)} on {name} has no completion progress flag assigned.");
            return;
        }

        if (GameProgressManager.Instance == null)
        {
            Debug.LogWarning("No GameProgressManager found in scene.");
            return;
        }

        GameProgressManager.Instance.AddProgressFlag(completionProgressFlag);
        onPuzzleCompleted?.Invoke();
        hasReported = true;
    }
}

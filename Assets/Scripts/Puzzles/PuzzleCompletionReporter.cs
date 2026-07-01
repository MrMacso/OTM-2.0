using UnityEngine;
using UnityEngine.Events;

public class PuzzleCompletionReporter : MonoBehaviour
{
    [SerializeField] private ProgressFlagReference completionProgressFlag = new ProgressFlagReference(ProgressFlags.BasementPuzzleSolved);
    [SerializeField] private bool reportOnlyOnce = true;
    [SerializeField] private UnityEvent onPuzzleCompleted;

    private bool hasReported;

    public void ReportCompleted()
    {
        if (hasReported && reportOnlyOnce)
        {
            return;
        }

        if (!completionProgressFlag.IsAssigned)
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

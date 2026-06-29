using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ChapterCompleteTrigger : MonoBehaviour
{
    [Header("Requirements")]
    [SerializeField] private string requiredFlag = ProgressFlags.CassetteEvidenceFound;
    [SerializeField] private string completionFlagToAdd = ProgressFlags.DemoCompleted;
    [SerializeField] private bool triggerOnlyOnce = true;

    [Header("UI")]
    [SerializeField] private GameObject completionPanel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI bodyText;
    [SerializeField] private string title = "Demo Complete";
    [TextArea]
    [SerializeField] private string body = "The evidence points to the basement. The next truth is below.";

    [Header("Feedback")]
    [SerializeField] private string feedbackMessage = "The evidence points to the basement.";
    [SerializeField] private bool showFeedbackMessage = true;

    [Header("Player")]
    [SerializeField] private PlayerControlStateController playerControl;
    [SerializeField] private bool lockPlayerControl = true;
    [SerializeField] private bool showCursor = true;
    [SerializeField] private bool pauseTime;

    [Header("Events")]
    [SerializeField] private UnityEvent onChapterCompleted;

    private GameProgressManager progressManager;
    private bool hasTriggered;
    private bool isSubscribed;

    private void Start()
    {
        if (completionPanel != null)
        {
            completionPanel.SetActive(false);
        }

        SubscribeToProgressManager();
        TryCompleteFromCurrentProgress();
    }

    private void OnEnable()
    {
        SubscribeToProgressManager();
        TryCompleteFromCurrentProgress();
    }

    private void OnDisable()
    {
        if (progressManager != null)
        {
            progressManager.ProgressFlagAdded -= HandleProgressFlagAdded;
        }

        isSubscribed = false;

        if (pauseTime)
        {
            Time.timeScale = 1f;
        }
    }

    private void SubscribeToProgressManager()
    {
        if (isSubscribed)
        {
            return;
        }

        progressManager = GameProgressManager.Instance;

        if (progressManager == null)
        {
            return;
        }

        progressManager.ProgressFlagAdded += HandleProgressFlagAdded;
        isSubscribed = true;
    }

    public void CompleteChapter()
    {
        if (hasTriggered && triggerOnlyOnce)
        {
            return;
        }

        hasTriggered = true;

        if (!string.IsNullOrWhiteSpace(completionFlagToAdd))
        {
            GameProgressManager.Instance?.AddProgressFlag(completionFlagToAdd);
        }

        if (showFeedbackMessage)
        {
            FeedbackMessageUI.Instance?.ShowDiscovery(feedbackMessage);
        }

        if (completionPanel != null)
        {
            completionPanel.SetActive(true);
        }

        if (titleText != null)
        {
            titleText.text = title;
        }

        if (bodyText != null)
        {
            bodyText.text = body;
        }

        if (lockPlayerControl && playerControl != null)
        {
            playerControl.SetCutsceneMode();
        }

        if (showCursor)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        if (pauseTime)
        {
            Time.timeScale = 0f;
        }

        onChapterCompleted?.Invoke();
    }

    public void HideCompletionPanel()
    {
        if (completionPanel != null)
        {
            completionPanel.SetActive(false);
        }

        if (pauseTime)
        {
            Time.timeScale = 1f;
        }

        if (lockPlayerControl && playerControl != null)
        {
            playerControl.SetGameplayMode();
        }
    }

    private void TryCompleteFromCurrentProgress()
    {
        if (string.IsNullOrWhiteSpace(requiredFlag))
        {
            return;
        }

        if (progressManager != null && progressManager.HasProgressFlag(requiredFlag))
        {
            CompleteChapter();
        }
    }

    private void HandleProgressFlagAdded(string flag)
    {
        if (flag == requiredFlag)
        {
            CompleteChapter();
        }
    }
}

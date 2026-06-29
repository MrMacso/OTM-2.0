using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class FeedbackMessageUI : MonoBehaviour
{
    public static FeedbackMessageUI Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI messageText;

    [Header("Timing")]
    [SerializeField] private float defaultDuration = 3f;

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color warningColor = new Color(1f, 0.82f, 0.35f);
    [SerializeField] private Color dangerColor = new Color(1f, 0.28f, 0.22f);
    [SerializeField] private Color discoveryColor = new Color(0.55f, 0.85f, 1f);

    [Header("Events")]
    [SerializeField] private UnityEvent onMessageShown;
    [SerializeField] private UnityEvent onMessageHidden;

    private float hideTimer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Duplicate FeedbackMessageUI found. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        HideImmediate();
    }

    private void Update()
    {
        if (hideTimer <= 0f)
        {
            return;
        }

        hideTimer -= Time.deltaTime;

        if (hideTimer <= 0f)
        {
            HideImmediate();
            onMessageHidden?.Invoke();
        }
    }

    public void ShowMessage(string message)
    {
        Show(message, defaultDuration, normalColor);
    }

    public void ShowWarning(string message)
    {
        Show(message, defaultDuration, warningColor);
    }

    public void ShowDanger(string message)
    {
        Show(message, defaultDuration, dangerColor);
    }

    public void ShowDiscovery(string message)
    {
        Show(message, defaultDuration, discoveryColor);
    }

    public void Show(string message, float duration, Color color)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        if (panel != null)
        {
            panel.SetActive(true);
        }

        if (messageText != null)
        {
            messageText.text = message;
            messageText.color = color;
        }

        hideTimer = duration > 0f ? duration : defaultDuration;
        onMessageShown?.Invoke();
    }

    public void HideImmediate()
    {
        hideTimer = 0f;

        if (messageText != null)
        {
            messageText.text = string.Empty;
        }

        if (panel != null)
        {
            panel.SetActive(false);
        }
    }
}

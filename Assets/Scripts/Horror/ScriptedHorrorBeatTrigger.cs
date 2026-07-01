using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ScriptedHorrorBeatTrigger : MonoBehaviour
{
    [Header("Trigger")]
    [SerializeField] private bool triggerOnPlayerEnter = true;
    [SerializeField] private bool triggerOnce = true;
    [SerializeField] private bool requireSpecificPeriod = true;
    [SerializeField] private MuseumTimePeriod requiredPeriod = MuseumTimePeriod.Past;
    [SerializeField] private ProgressFlagReference requiredFlag;
    [SerializeField] private ProgressFlagReference blockedByFlag = new ProgressFlagReference(ProgressFlags.FirstJumpscareTriggered);
    [SerializeField] private ProgressFlagReference completionFlag = new ProgressFlagReference(ProgressFlags.FirstJumpscareTriggered);

    [Header("Beat Timing")]
    [SerializeField] private float preDelay = 0f;
    [SerializeField] private float scareDuration = 1.5f;
    [SerializeField] private float panicDuration = 4f;

    [Header("Pulse Pressure")]
    [SerializeField] private float pulseSpike = 28f;
    [SerializeField] private bool startPanic = true;
    [SerializeField] private bool endPanicAfterDuration = true;

    [Header("Feedback")]
    [SerializeField] private string scareMessage = "Something moved in the dark.";

    [Header("Objects")]
    [SerializeField] private bool hideScareObjectsOnStart = true;
    [SerializeField] private GameObject[] objectsToShowDuringScare;
    [SerializeField] private GameObject[] objectsToHideDuringScare;

    [Header("Audio")]
    [SerializeField] private AudioSource scareAudio;

    [Header("Camera")]
    [SerializeField] private CameraShake cameraShake;
    [SerializeField] private bool shakeCamera = true;

    [Header("Events")]
    [SerializeField] private UnityEvent onBeatStarted;
    [SerializeField] private UnityEvent onScareShown;
    [SerializeField] private UnityEvent onScareHidden;
    [SerializeField] private UnityEvent onBeatFinished;

    private bool hasTriggered;
    private Coroutine beatRoutine;

    private void Start()
    {
        if (hideScareObjectsOnStart)
        {
            SetObjectsActive(objectsToShowDuringScare, false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!triggerOnPlayerEnter || !other.CompareTag("Player"))
        {
            return;
        }

        TriggerBeat();
    }

    public void TriggerBeat()
    {
        if (beatRoutine != null)
        {
            return;
        }

        if (!CanTrigger())
        {
            return;
        }

        beatRoutine = StartCoroutine(BeatRoutine());
    }

    private IEnumerator BeatRoutine()
    {
        hasTriggered = true;
        onBeatStarted?.Invoke();

        if (preDelay > 0f)
        {
            yield return new WaitForSeconds(preDelay);
        }

        FeedbackMessageUI.Instance?.ShowDanger(scareMessage);
        WatchPulseSystem.Instance?.AddPulse(pulseSpike);

        if (startPanic)
        {
            WatchPulseSystem.Instance?.StartPanic();
        }

        if (scareAudio != null)
        {
            scareAudio.Play();
        }

        if (shakeCamera && cameraShake != null)
        {
            cameraShake.Shake();
        }

        SetObjectsActive(objectsToShowDuringScare, true);
        SetObjectsActive(objectsToHideDuringScare, false);
        onScareShown?.Invoke();

        if (scareDuration > 0f)
        {
            yield return new WaitForSeconds(scareDuration);
        }

        SetObjectsActive(objectsToShowDuringScare, false);
        SetObjectsActive(objectsToHideDuringScare, true);
        onScareHidden?.Invoke();

        if (panicDuration > scareDuration)
        {
            yield return new WaitForSeconds(panicDuration - scareDuration);
        }

        if (endPanicAfterDuration)
        {
            WatchPulseSystem.Instance?.EndPanic();
        }

        if (completionFlag.IsAssigned)
        {
            GameProgressManager.Instance?.AddProgressFlag(completionFlag);
        }

        onBeatFinished?.Invoke();
        beatRoutine = null;
    }

    private bool CanTrigger()
    {
        if (hasTriggered && triggerOnce)
        {
            return false;
        }

        if (requireSpecificPeriod)
        {
            if (MuseumTimelineManager.Instance == null ||
                MuseumTimelineManager.Instance.CurrentPeriod != requiredPeriod)
            {
                return false;
            }
        }

        if (requiredFlag.IsAssigned &&
            (GameProgressManager.Instance == null || !GameProgressManager.Instance.HasProgressFlag(requiredFlag)))
        {
            return false;
        }

        if (blockedByFlag.IsAssigned &&
            GameProgressManager.Instance != null &&
            GameProgressManager.Instance.HasProgressFlag(blockedByFlag))
        {
            return false;
        }

        return true;
    }

    private void SetObjectsActive(GameObject[] objects, bool active)
    {
        if (objects == null)
        {
            return;
        }

        foreach (GameObject target in objects)
        {
            if (target != null)
            {
                target.SetActive(active);
            }
        }
    }
}

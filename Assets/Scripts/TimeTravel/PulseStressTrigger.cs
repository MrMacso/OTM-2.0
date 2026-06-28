using UnityEngine;

public class PulseStressTrigger : MonoBehaviour
{
    [SerializeField] private float pulseSpike = 25f;
    [SerializeField] private bool startPanic;
    [SerializeField] private bool endPanicOnExit;
    [SerializeField] private bool triggerOnce = true;

    private bool hasTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered && triggerOnce)
        {
            return;
        }

        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (WatchPulseSystem.Instance == null)
        {
            Debug.LogWarning("PulseStressTrigger could not find WatchPulseSystem.");
            return;
        }

        WatchPulseSystem.Instance.AddPulse(pulseSpike);

        if (startPanic)
        {
            WatchPulseSystem.Instance.StartPanic();
        }

        hasTriggered = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!endPanicOnExit || !other.CompareTag("Player"))
        {
            return;
        }

        WatchPulseSystem.Instance?.EndPanic();
    }
}

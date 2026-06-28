using UnityEngine;

public class BreathingZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            WatchPulseSystem.Instance?.SetBreathing(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            WatchPulseSystem.Instance?.SetBreathing(false);
        }
    }
}

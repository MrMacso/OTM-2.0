using UnityEngine;
public class EventDirector : MonoBehaviour
{
    [SerializeField] private float minTimeBetweenEvents = 20f;
    [SerializeField] private float maxTimeBetweenEvents = 60f;
    private float eventTimer;
    private float nextEventTime;
    private void Start()
    {
        ScheduleNextEvent();
    }
    private void Update()
    {
        eventTimer += Time.deltaTime;
        if (eventTimer >= nextEventTime)
        {
            TriggerRandomEvent();
            ScheduleNextEvent();
        }
    }
    private void ScheduleNextEvent()
    {
        eventTimer = 0f;
        nextEventTime = Random.Range(minTimeBetweenEvents, maxTimeBetweenEvents);
    }
    private void TriggerRandomEvent()
    {
        Debug.Log("Trigger horror event.");
        // Later:
        // Pick from a list of possible scare events.
        // Avoid repeating the same event too often.
        // Increase intensity as game progresses.
    }
}

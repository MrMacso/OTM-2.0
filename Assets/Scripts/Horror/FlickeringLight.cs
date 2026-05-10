using System.Collections;
using UnityEngine;
public class FlickeringLight : MonoBehaviour
{
    [SerializeField] private Light targetLight;
    [SerializeField] private float minDelay = 0.05f;
    [SerializeField] private float maxDelay = 0.3f;
    [SerializeField] private bool startOnAwake = true;
    private Coroutine flickerRoutine;
    private void Start()
    {
        if (startOnAwake)
        {
            StartFlicker();
        }
    }
    public void StartFlicker()
    {
        if (flickerRoutine == null)
        {
            flickerRoutine = StartCoroutine(Flicker());
        }
    }
    public void StopFlicker()
    {
        if (flickerRoutine != null)
        {
            StopCoroutine(flickerRoutine);
            flickerRoutine = null;
        }
        targetLight.enabled = true;
    }
    private IEnumerator Flicker()
    {
        while (true)
        {
            targetLight.enabled = !targetLight.enabled;
            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
        }
    }
}

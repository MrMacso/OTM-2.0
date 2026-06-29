using System.Collections;
using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    [SerializeField] private Light targetLight;
    [SerializeField] private float minDelay = 0.05f;
    [SerializeField] private float maxDelay = 0.3f;
    [SerializeField] private bool startOnAwake = true;

    private Coroutine flickerRoutine;

    private void Awake()
    {
        if (targetLight == null)
        {
            targetLight = GetComponent<Light>();
        }
    }

    private void Start()
    {
        if (targetLight == null)
        {
            Debug.LogWarning($"{nameof(FlickeringLight)} on {name} has no target light assigned.");
            enabled = false;
            return;
        }

        if (startOnAwake)
        {
            StartFlicker();
        }
    }

    public void StartFlicker()
    {
        if (targetLight == null)
        {
            return;
        }

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

        if (targetLight != null)
        {
            targetLight.enabled = true;
        }
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

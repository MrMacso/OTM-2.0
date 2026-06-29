using System.Collections;
using UnityEngine;

public class LightFlickerBurst : MonoBehaviour
{
    [SerializeField] private Light[] lightsToFlicker;
    [SerializeField] private float duration = 1.25f;
    [SerializeField] private float minDelay = 0.04f;
    [SerializeField] private float maxDelay = 0.16f;
    [SerializeField] private bool restoreLightsOnFinish = true;

    private Coroutine flickerRoutine;
    private bool[] originalStates;

    public void Play()
    {
        if (lightsToFlicker == null || lightsToFlicker.Length == 0)
        {
            return;
        }

        if (flickerRoutine != null)
        {
            StopCoroutine(flickerRoutine);
        }

        flickerRoutine = StartCoroutine(FlickerRoutine());
    }

    private IEnumerator FlickerRoutine()
    {
        CacheOriginalStates();

        float timer = 0f;

        while (timer < duration)
        {
            ToggleLights();
            float wait = Random.Range(minDelay, maxDelay);
            timer += wait;
            yield return new WaitForSeconds(wait);
        }

        if (restoreLightsOnFinish)
        {
            RestoreOriginalStates();
        }

        flickerRoutine = null;
    }

    private void CacheOriginalStates()
    {
        originalStates = new bool[lightsToFlicker.Length];

        for (int i = 0; i < lightsToFlicker.Length; i++)
        {
            originalStates[i] = lightsToFlicker[i] != null && lightsToFlicker[i].enabled;
        }
    }

    private void ToggleLights()
    {
        foreach (Light targetLight in lightsToFlicker)
        {
            if (targetLight != null)
            {
                targetLight.enabled = !targetLight.enabled;
            }
        }
    }

    private void RestoreOriginalStates()
    {
        if (originalStates == null)
        {
            return;
        }

        for (int i = 0; i < lightsToFlicker.Length; i++)
        {
            if (lightsToFlicker[i] != null)
            {
                lightsToFlicker[i].enabled = originalStates[i];
            }
        }
    }
}

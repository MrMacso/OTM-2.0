using System.Collections;
using UnityEngine;
public class JumpScareTrigger : MonoBehaviour
{
    [SerializeField] private GameObject scareObject;
    [SerializeField] private AudioSource scareAudio;
    [SerializeField] private float scareDuration = 1f;
    [SerializeField] private bool triggerOnce = true;
    private bool hasTriggered;
    private void Start()
    {
        if (scareObject != null)
        {
            scareObject.SetActive(false);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered && triggerOnce) return;
        if (other.CompareTag("Player"))
        {
            StartCoroutine(TriggerScare());
        }
    }
    private IEnumerator TriggerScare()
    {
        hasTriggered = true;
        if (scareObject != null)
        {
            scareObject.SetActive(true);
        }
        if (scareAudio != null)
        {
            scareAudio.Play();
        }
        yield return new WaitForSeconds(scareDuration);
        if (scareObject != null)
        {
            scareObject.SetActive(false);
        }
    }
}

using Unity.Cinemachine;
using UnityEngine;

public class ShakeDemo : MonoBehaviour
{
    [SerializeField] private CameraShake cameraShake;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            cameraShake.Shake();
            Debug.Log("Player entered the field");
        }
    }
}

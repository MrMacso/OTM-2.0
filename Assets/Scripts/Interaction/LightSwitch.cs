using UnityEngine;

public class LightSwitch : MonoBehaviour, IInteractable
{
    [SerializeField] private Light targetLight;
    [SerializeField] private string prompt = "Press E to toggle light";

    public string InteractionPrompt => prompt;

    private void Start()
    {
        if (targetLight == null)
        {
            Debug.LogWarning($"{nameof(LightSwitch)} on {name} has no target light assigned.");
            enabled = false;
            return;
        }

        targetLight.enabled = false;
    }

    public void Interact(GameObject player)
    {
        if (targetLight == null)
        {
            return;
        }

        targetLight.enabled = !targetLight.enabled;
    }
}

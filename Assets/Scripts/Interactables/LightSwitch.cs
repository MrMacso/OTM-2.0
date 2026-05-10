using UnityEngine;
public class LightSwitch : MonoBehaviour, IInteractable
{
    [SerializeField] private Light targetLight;
    [SerializeField] private string prompt = "Press E to toggle light";

    private void Start()
    {
        targetLight.enabled = false;
    }
    public string GetPrompt()
    {
        return prompt;
    }
    public void Interact(PlayerInteraction player)
    {
        targetLight.enabled = !targetLight.enabled;
    }
}

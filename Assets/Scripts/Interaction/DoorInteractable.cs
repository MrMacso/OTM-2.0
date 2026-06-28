using UnityEngine;

public class DoorInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform doorPivot;
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 5f;

    private bool isOpen;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    public string InteractionPrompt => isOpen ? "Press E to close door" : "Press E to open door";

    private void Start()
    {
        if (doorPivot == null)
        {
            Debug.LogWarning($"{nameof(DoorInteractable)} on {name} has no door pivot assigned.");
            enabled = false;
            return;
        }

        closedRotation = doorPivot.localRotation;
        openRotation = Quaternion.Euler(
            doorPivot.localEulerAngles.x,
            doorPivot.localEulerAngles.y + openAngle,
            doorPivot.localEulerAngles.z
        );
    }

    private void Update()
    {
        Quaternion targetRotation = isOpen ? openRotation : closedRotation;
        doorPivot.localRotation = Quaternion.Slerp(
            doorPivot.localRotation,
            targetRotation,
            openSpeed * Time.deltaTime
        );
    }

    public void Interact(GameObject player)
    {
        isOpen = !isOpen;
    }
}

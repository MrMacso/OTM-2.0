using UnityEngine;
public class DoorInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform doorPivot;
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 5f;
    private bool isOpen;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private void Start()
    {
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
    public string GetPrompt()
    {
        return isOpen ? "Press E to close door" : "Press E to open door";
    }
    public void Interact(PlayerInteraction player)
    {
        isOpen = !isOpen;
    }
}

using System;
using UnityEngine;

public class PlayerControlStateController : MonoBehaviour
{
    [Header("Controlled Components")]
    [Tooltip("Movement scripts that should be disabled when the player cannot move.")]
    [SerializeField] private MonoBehaviour[] movementScripts;

    [Tooltip("Camera/look scripts that should be disabled when the player cannot look.")]
    [SerializeField] private MonoBehaviour[] lookScripts;

    [Tooltip("Interaction scripts that should be disabled when the player cannot interact.")]
    [SerializeField] private MonoBehaviour[] interactionScripts;

    public PlayerControlMode CurrentMode { get; private set; } = PlayerControlMode.Gameplay;

    public bool CanMove { get; private set; } = true;
    public bool CanLook { get; private set; } = true;
    public bool CanInteract { get; private set; } = true;

    public event Action<PlayerControlMode> ControlModeChanged;

    private void Start()
    {
        ApplyMode(PlayerControlMode.Gameplay);
    }

    public void SetGameplayMode()
    {
        ApplyMode(PlayerControlMode.Gameplay);
    }

    public void SetInspectingMode()
    {
        ApplyMode(PlayerControlMode.Inspecting);
    }

    public void SetDialogueMode()
    {
        ApplyMode(PlayerControlMode.Dialogue);
    }

    public void SetGuidebookMode()
    {
        ApplyMode(PlayerControlMode.Guidebook);
    }

    public void SetPuzzleMode()
    {
        ApplyMode(PlayerControlMode.Puzzle);
    }

    public void SetCutsceneMode()
    {
        ApplyMode(PlayerControlMode.Cutscene);
    }

    public void SetJumpscareMode()
    {
        ApplyMode(PlayerControlMode.Jumpscare);
    }

    public void SetTimeTravelMode()
    {
        ApplyMode(PlayerControlMode.TimeTravel);
    }

    private void ApplyMode(PlayerControlMode mode)
    {
        CurrentMode = mode;

        switch (mode)
        {
            case PlayerControlMode.Gameplay:
                CanMove = true;
                CanLook = true;
                CanInteract = true;
                SetCursor(false);
                break;

            case PlayerControlMode.Inspecting:
            case PlayerControlMode.Puzzle:
            case PlayerControlMode.Guidebook:
                CanMove = false;
                CanLook = false;
                CanInteract = false;
                SetCursor(true);
                break;

            case PlayerControlMode.Dialogue:
                CanMove = false;
                CanLook = true;
                CanInteract = false;
                SetCursor(false);
                break;

            case PlayerControlMode.Cutscene:
            case PlayerControlMode.Jumpscare:
            case PlayerControlMode.TimeTravel:
                CanMove = false;
                CanLook = false;
                CanInteract = false;
                SetCursor(false);
                break;

            default:
                CanMove = true;
                CanLook = true;
                CanInteract = true;
                SetCursor(false);
                break;
        }

        ApplyComponentStates();
        ControlModeChanged?.Invoke(CurrentMode);
    }

    private void ApplyComponentStates()
    {
        SetComponentsEnabled(movementScripts, CanMove);
        SetComponentsEnabled(lookScripts, CanLook);
        SetComponentsEnabled(interactionScripts, CanInteract);
    }

    private void SetComponentsEnabled(MonoBehaviour[] components, bool isEnabled)
    {
        if (components == null)
        {
            return;
        }

        foreach (MonoBehaviour component in components)
        {
            if (component != null)
            {
                component.enabled = isEnabled;
            }
        }
    }

    private void SetCursor(bool visible)
    {
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
    }
}

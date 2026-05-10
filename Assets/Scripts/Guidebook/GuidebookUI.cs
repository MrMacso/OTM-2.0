using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
public class GuidebookUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject guidebookPanel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI bodyText;
    [SerializeField] private Image pageImage;
    [Header("Pages")]
    [SerializeField] private List<GuidebookPage> pages = new();
    [Header("Player")]
    [SerializeField] private InputSystemFPSController playerController;
    private PlayerInputActions inputActions;
    private int currentPageIndex;
    private bool isOpen;
    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }
    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Guidebook.performed += OnToggleGuidebook;
    }
    private void OnDisable()
    {
        inputActions.Player.Guidebook.performed -= OnToggleGuidebook;
        inputActions.Player.Disable();
    }
    private void Start()
    {
        guidebookPanel.SetActive(false);
        ShowPage(0);
    }
    private void Update()
    {
        if (!isOpen) return;
        if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            NextPage();
        }
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            PreviousPage();
        }
    }
    private void OnToggleGuidebook(InputAction.CallbackContext context)
    {
        ToggleGuidebook();
    }
    private void ToggleGuidebook()
    {
        isOpen = !isOpen;
        guidebookPanel.SetActive(isOpen);
        Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isOpen;
        Time.timeScale = isOpen ? 0f : 1f;
        if (playerController != null)
        {
            playerController.SetLookEnabled(!isOpen);
        }
    }
    private void ShowPage(int index)
    {
        if (pages.Count == 0) return;
        currentPageIndex = Mathf.Clamp(index, 0, pages.Count - 1);
        GuidebookPage page = pages[currentPageIndex];
        titleText.text = page.pageTitle;
        bodyText.text = page.pageText;
        if (pageImage != null)
        {
            pageImage.sprite = page.pageImage;
            pageImage.enabled = page.pageImage != null;
        }
    }
    private void NextPage()
    {
        ShowPage(currentPageIndex + 1);
    }
    private void PreviousPage()
    {
        ShowPage(currentPageIndex - 1);
    }
}
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InventoryUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private TextMeshProUGUI itemListText;
    [SerializeField] private TextMeshProUGUI selectedItemTitleText;
    [SerializeField] private TextMeshProUGUI selectedItemDescriptionText;
    [SerializeField] private TextMeshProUGUI selectedItemReadableText;

    [Header("Player")]
    [SerializeField] private PlayerControlStateController playerControl;

    [Header("Input")]
    [SerializeField] private bool useKeyboardShortcut = true;
    [SerializeField] private Key toggleKey = Key.I;

    [Header("Events")]
    [SerializeField] private UnityEvent onOpened;
    [SerializeField] private UnityEvent onClosed;
    [SerializeField] private UnityEvent onSelectionChanged;

    private bool isSubscribed;
    private bool isOpen;
    private int selectedIndex;

    private void OnEnable()
    {
        SubscribeToInventory();
    }

    private void OnDisable()
    {
        if (Inventory.Instance != null)
        {
            Inventory.Instance.InventoryChanged -= Refresh;
        }

        isSubscribed = false;

        if (isOpen)
        {
            isOpen = false;

            if (playerControl != null)
            {
                playerControl.SetGameplayMode();
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }

            Time.timeScale = 1f;
        }
    }

    private void Start()
    {
        SubscribeToInventory();

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }

        Refresh();
    }

    private void SubscribeToInventory()
    {
        if (isSubscribed || Inventory.Instance == null)
        {
            return;
        }

        Inventory.Instance.InventoryChanged += Refresh;
        isSubscribed = true;
    }

    private void Update()
    {
        if (useKeyboardShortcut && Keyboard.current != null && Keyboard.current[toggleKey].wasPressedThisFrame)
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        if (isOpen)
        {
            CloseInventory();
        }
        else
        {
            OpenInventory();
        }
    }

    public void OpenInventory()
    {
        isOpen = true;

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(true);
        }

        if (playerControl != null)
        {
            playerControl.SetGuidebookMode();
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        Refresh();
        onOpened?.Invoke();
    }

    public void CloseInventory()
    {
        isOpen = false;

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }

        if (playerControl != null)
        {
            playerControl.SetGameplayMode();
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        onClosed?.Invoke();
    }

    public void SelectNext()
    {
        if (Inventory.Instance == null || Inventory.Instance.Entries.Count == 0)
        {
            return;
        }

        selectedIndex = (selectedIndex + 1) % Inventory.Instance.Entries.Count;
        RefreshDetails();
        onSelectionChanged?.Invoke();
    }

    public void SelectPrevious()
    {
        if (Inventory.Instance == null || Inventory.Instance.Entries.Count == 0)
        {
            return;
        }

        selectedIndex--;

        if (selectedIndex < 0)
        {
            selectedIndex = Inventory.Instance.Entries.Count - 1;
        }

        RefreshDetails();
        onSelectionChanged?.Invoke();
    }

    public void Refresh()
    {
        RefreshList();
        RefreshDetails();
    }

    private void RefreshList()
    {
        if (itemListText == null)
        {
            return;
        }

        if (Inventory.Instance == null || Inventory.Instance.Entries.Count == 0)
        {
            itemListText.text = "Inventory is empty.";
            return;
        }

        selectedIndex = Mathf.Clamp(selectedIndex, 0, Inventory.Instance.Entries.Count - 1);
        StringBuilder builder = new StringBuilder();

        for (int i = 0; i < Inventory.Instance.Entries.Count; i++)
        {
            InventoryEntry entry = Inventory.Instance.Entries[i];

            if (entry == null || entry.item == null)
            {
                continue;
            }

            string selector = i == selectedIndex ? "> " : "  ";
            string quantity = entry.quantity > 1 ? $" x{entry.quantity}" : string.Empty;
            builder.AppendLine(selector + entry.item.DisplayName + quantity);
        }

        itemListText.text = builder.ToString();
    }

    private void RefreshDetails()
    {
        InventoryItemDefinition item = GetSelectedItem();

        if (selectedItemTitleText != null)
        {
            selectedItemTitleText.text = item != null ? item.DisplayName : string.Empty;
        }

        if (selectedItemDescriptionText != null)
        {
            selectedItemDescriptionText.text = item != null ? item.Description : string.Empty;
        }

        if (selectedItemReadableText != null)
        {
            selectedItemReadableText.text = item != null ? item.ReadableText : string.Empty;
        }
    }

    private InventoryItemDefinition GetSelectedItem()
    {
        if (Inventory.Instance == null || Inventory.Instance.Entries.Count == 0)
        {
            return null;
        }

        selectedIndex = Mathf.Clamp(selectedIndex, 0, Inventory.Instance.Entries.Count - 1);
        InventoryEntry entry = Inventory.Instance.Entries[selectedIndex];
        return entry != null ? entry.item : null;
    }
}

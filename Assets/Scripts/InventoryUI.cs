using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Inventory inventory;
    public Transform itemsParent;
    public GameObject inventorySlotPrefab;
    public GameObject player;

    // Refresh the UI when the inventory changes
    public void UpdateUI()
    {
        // Clear existing slots
        foreach (Transform child in itemsParent)
        {
            Destroy(child.gameObject);
        }

        // Recreate slots for current inventory items
        foreach (Item item in inventory.items)
        {
            GameObject slot = Instantiate(inventorySlotPrefab, itemsParent);
            Image icon = slot.GetComponent<Image>();
            icon.sprite = item.icon;

            Button button = slot.GetComponent<Button>();
            button.onClick.AddListener(() => UseItemFromUI(item));
        }
    }

    // Handle item use from UI
    private void UseItemFromUI(Item item)
    {
        inventory.UseItem(item, player); // Use the item
        UpdateUI();                      // Refresh the UI
    }

    private void OnEnable()
    {
        UpdateUI(); // Refresh UI when inventory UI is opened
    }
}

using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<Item> items = new List<Item>();
    public int maxSlots = 20;

    public bool AddItem(Item item)
    {
        if (items.Count >= maxSlots)
        {
            Debug.Log("Inventory is full!");
            return false;
        }

        items.Add(item);
        Debug.Log(item.itemName + " added to inventory.");
        return true;
    }

    public void RemoveItem(Item item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            Debug.Log(item.itemName + " removed from inventory.");
        }
    }
    public void UseItem(Item item, GameObject user)
    {
        if (items.Contains(item))
        {
            item.Use(user);
            items.Remove(item); // Remove after use if it's not reusable
            Debug.Log("Used item: " + item.itemName);
        }
        else
        {
            Debug.Log("Item not in inventory!");
        }
    }
}

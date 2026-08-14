using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance;

    public List<string> items = new List<string>();
    public InventoryUI inventoryUI;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (inventoryUI != null)
            inventoryUI.UpdateList(items);
    }

    public void AddItem(string itemName)
    {
        items.Add(itemName);
        if (inventoryUI != null)
            inventoryUI.UpdateList(items);
        Debug.Log($"Added: {itemName}");
    }

    public bool HasItem(string itemName)
    {
        return items.Contains(itemName);
    }

    public void RemoveItem(string itemName)
    {
        if (items.Contains(itemName))
        {
            items.Remove(itemName);
            if (inventoryUI != null)
                inventoryUI.UpdateList(items);
            Debug.Log($"Removed: {itemName}");
        }
    }

    public void RemoveItemAt(int index)
    {
        if (index >= 0 && index < items.Count)
        {
            items.RemoveAt(index);
            if (inventoryUI != null)
                inventoryUI.UpdateList(items);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (inventoryUI != null)
                inventoryUI.Show();
        }
    }
}
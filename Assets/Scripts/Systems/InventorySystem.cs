using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class InventorySystem
{
    public int MaxItems;
    public ItemDatabase GameItems;
    public EquipmentDatabase GameEquipment;
    public ConsumableDatabase GameConsumables;

    public List<Item> PlayerItems;

    public InventorySystem(int maxItems = 16)
    {
        PlayerItems = new List<Item>();
        MaxItems = maxItems;

        GameEquipment = GameManager.Instance.GameEquipment;
        GameItems = GameManager.Instance.ItemDatabase;
        GameConsumables = GameManager.Instance.ConsumableDatabase;
    }

    public void SetEquipmentDetails(Item item,Button selected)
    {
        GameManager.Instance.ItemDetailsPanel.SetEquipment(item, selected);
    }

    public void SetItemDetails(Item item, Button selected)
    {
        GameManager.Instance.ItemDetailsPanel.SetItem(item, selected);
    }

    public void ConsumeItem(ConsumableItem item)
    {
        GameManager.Player.Consume(item);
    }

    public void EquipItem(EquipmentItem item)
    {
        GameManager.Player.EquipItem(item);
    }

    private bool AddNewItem(string identifiedName, ItemType type)
    {
        if (PlayerItems.Count < GameManager.Player.capacity.FinalValue)
        {
            if (type == ItemType.Item)
            {
                if (AddItem((Item)GameItems.GetItem(identifiedName)))
                    return true;
            }

            if (type == ItemType.Equipment)
            {
                if (AddItem((EquipmentItem)GameEquipment.GetEquipment(identifiedName)))
                    return true;
            }

            if (type == ItemType.Consumable)
            {
                if (AddItem((ConsumableItem)GameConsumables.GetItem(identifiedName)))
                    return true;
            }
        }
        else
        {
            Debug.LogError("Inventory Full.... Item slipped though");
            return false;
        }

        return false;
    }

    public bool AddMultiItems(string name, ItemType type, int number)
    {
        for (int i = 0; i < number; i++)
        {
            if (AddNewItem(name, type))
                continue;
            else
                return false;
        }

        return true;
    }

    public bool AddItem(string identifiedName, ItemType type)
    {
        return AddNewItem(identifiedName, type);
    }

    public bool AddItem(EquipmentItem item)
    {
        EquipmentItem temp = item;
        UIEventHandler.ItemAddedToInventory(temp);
        if (temp == null)
            return false;
        else return true;

    }

    public bool AddItem(Item item)
    {
        Item temp = item;
        UIEventHandler.ItemAddedToInventory(temp);
        if (temp == null)
            return false;
        else return true;
    }

    public bool AddItem(ConsumableItem item)
    {
        ConsumableItem temp = item;
        UIEventHandler.ItemAddedToInventory(temp);
        if (temp == null)
            return false;
        else return true;
    }

    public bool RemoveItem(Item item)
    {
        Item temp = item;
        UIEventHandler.ItemRemovedFromInventory(item);
        if (temp == null)
            return false;
        else return true;
    }
}

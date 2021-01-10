using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPanel : MonoBehaviour
{
    public static InventoryPanel Instance;
    public ItemDisplayObject ItemObject;
    public Transform InventoryContent;
    List<ItemDisplayObject> itemObjectList = new List<ItemDisplayObject>();
    Item currentSelectedItem { get; set; }

    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance.gameObject);


        Instance = this;

        UIEventHandler.OnItemAddedToInventory += ItemAdded;
        UIEventHandler.OnItemRemovedFromInventory += ItemRemoved;
    }

    private void OnApplicationQuit()
    {
        UIEventHandler.OnItemAddedToInventory -= ItemAdded;
    }

    private int GetItemIndex(Item item)
    {
        for( int i = 0; i < itemObjectList.Count; i++)
        {
            if(itemObjectList[i].Item == item)
            {
                return i;
            }
        }

        return -1;
    }

    public ItemDisplayObject CheckForItem(Item item)
    {
        foreach(ItemDisplayObject obj in itemObjectList)
        {
            if (obj.Item == item && item.Stackable)
                return obj;
        }

        return null;
    }

    public void ItemAdded(Item item)
    {
        ItemDisplayObject itemInInventory = CheckForItem(item);
        if (itemInInventory != null)
        {
            if(itemInInventory.stackAmount < 1)
            {
                itemInInventory.stackAmount = 1;
            }

            itemInInventory.stackAmount++;
            itemInInventory.SetItem(item);

        }
        else
        {
            ItemDisplayObject emptyItem = Instantiate(ItemObject);
            itemObjectList.Add(emptyItem);
            emptyItem.transform.SetParent(InventoryContent);
            emptyItem.transform.localScale = new Vector3(1, 1, 1);
            if (item.Stackable)
                emptyItem.stackAmount = 1;
            emptyItem.SetItem(item);
            GameManager.InventorySystem.PlayerItems.Add(item);
        }
    }


    public void ItemRemoved(Item item)
    {
        if (itemObjectList[GetItemIndex(item)].stackAmount > 1)
        {
            itemObjectList[GetItemIndex(item)].stackAmount--;
            itemObjectList[GetItemIndex(item)].SetItem(item);
        }
        else
        {
            Destroy(itemObjectList[GetItemIndex(item)].gameObject);
            GameManager.InventorySystem.PlayerItems.Remove(item);
            itemObjectList.RemoveAt(GetItemIndex(item));
        }
    }
}

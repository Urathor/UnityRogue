using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDisplayObject : MonoBehaviour
{
    public Item Item;
    public Text ItemText;
    public Text StackText;
    public Image ItemImage;
    public int stackAmount = 1;

    public void SetItem(Item item)
    {
        this.Item = item;
        SetupItemValues();
    }

    void SetupItemValues()
    {
        ItemText.text  = Item.IsIdentified ? Item.IdentifiedName : Item.ItemName;
        ItemImage.sprite = Item.ItemIcon;
        if (Item.Stackable)
            StackText.text = stackAmount.ToString();
        else
            StackText.text = "";
    }

    public void OnSelectItemButton()
    {
        GameManager.InventorySystem.SetItemDetails(Item, GetComponent<Button>());
    }
}

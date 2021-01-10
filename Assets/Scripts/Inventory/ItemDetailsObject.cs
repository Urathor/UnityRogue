using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDetailsObject : MonoBehaviour
{
    public Text StatText;
    Item item;
    public Button selectedItemButton, itemInteractButton;
    public Text itemNameText, itemDescriptionText, itemInteractButtonText;
    
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void SetEquipment(Item item , Button selectedButton)
    {
        gameObject.SetActive(true);
        StatText.text = "";

        if (item.IsIdentified)
        {
            if (item is EquipmentItem)
            {
                foreach (StatModifier mod in (item as EquipmentItem).StatModifiers)
                {
                    StatText.text += mod.ModifiedStat.name + ": " + mod.Value + "\n";
                }

            }
        }
        itemInteractButton.gameObject.SetActive(true);
        itemInteractButton.onClick.RemoveAllListeners();
        this.item = item;
        selectedItemButton = selectedButton;
        itemNameText.text = item.ItemName;
        itemDescriptionText.text = item.Description;
        itemInteractButtonText.text = "Unequip";
        itemInteractButton.onClick.AddListener(UnequipPlayer);
    }

    public void SetItem(Item item, Button selectedButton)
    {
        gameObject.SetActive(true);
        StatText.text = "";

        if(item.IsIdentified)
        {
            if(item is EquipmentItem)
            {
                foreach(StatModifier mod in (item as EquipmentItem).StatModifiers)
                {
                    StatText.text += mod.ModifiedStat.name + ": " + mod.Value + "\n";
                }
                
            }
        }
        itemInteractButton.gameObject.SetActive(true);
        itemInteractButton.onClick.RemoveAllListeners();
        this.item = item;
        selectedItemButton = selectedButton;
        itemNameText.text = item.ItemName;
        itemDescriptionText.text = item.Description;
        if (item is EquipmentItem)
            itemInteractButtonText.text = "Equip";
        else if (item is ConsumableItem)
            itemInteractButtonText.text = "Consume";
        else
            itemInteractButtonText.text = "Close";

        itemInteractButton.onClick.AddListener(OnItemInteract);
    }

    public void UnequipPlayer()
    {
        GameManager.Player.UnequipItem((EquipmentItem)item);
        selectedItemButton.GetComponent<EquipmentDisplayItem>().SetEmpty();
        selectedItemButton = null;
        ClosePanel();
    }

    public void ClosePanel()
    {

        item = null;
        gameObject.SetActive(false);
    }

    public void OnItemInteract()
    {
        if (item is ConsumableItem)
        {
            GameManager.InventorySystem.ConsumeItem((ConsumableItem)item);
            GameManager.InventorySystem.RemoveItem(item);
        }
        else if (item is EquipmentItem)
        {
            GameManager.InventorySystem.EquipItem((EquipmentItem)item);
            GameManager.InventorySystem.RemoveItem(item);
        }

        ClosePanel();
    }

    public void OnDropItem()
    {
        if (item != null)
        {
            GameObject lootObject = Instantiate(Res.Instance.Loot, new Vector2(GameManager.Player.X, GameManager.Player.Y), Quaternion.identity);
            Loot loot = lootObject.GetComponent<Loot>();
            loot.X = GameManager.Player.X;
            loot.Y = GameManager.Player.Y;
            loot.go = lootObject;
            lootObject.transform.SetParent(GameManager.ObjectHolder);
            loot.ItemName = item.IdentifiedName;
            loot.Type = item.Type;
            loot.Renderer.sprite = item.ItemIcon;
            GameManager.DungeonMap.AddLoot(loot);
        }

        if (GameManager.InventorySystem.RemoveItem(item))
        {
            Destroy(selectedItemButton.gameObject);
        }

        ClosePanel();
    }

}

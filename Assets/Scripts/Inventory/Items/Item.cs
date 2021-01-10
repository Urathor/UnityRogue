using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Item,
    Equipment,
    Consumable
}

[System.Serializable]
public class Item
{
    [SerializeField] public ItemType Type;
    [SerializeField] public string ItemName;
    [SerializeField] public string IdentifiedName;
    [SerializeField] public string Description;
    [SerializeField] public int GoldValue;
    [SerializeField] public Sprite ItemIcon;
    [SerializeField] public bool IsIdentified;
    [SerializeField] public bool Stackable;

    public Item()
    {

    }
    public Item(string name, Sprite icon, bool stackable = true)
    {
        ItemName = name;
        ItemIcon = icon;
        Stackable = stackable;
    }
}

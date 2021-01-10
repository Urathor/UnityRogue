using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueSharp;
using RogueSharp.Random;

[System.Serializable]
public class LootGenerator
{
    public List<LootDrop> LootList;

    public Item GetDrop()
    {
        
        int roll = Random.Range(1, 101);
        int weightSum = 0;

        foreach(LootDrop drop in LootList)
        {
            weightSum += drop.Weight;
            if(roll < weightSum)
            {
                if (drop.Type == ItemType.Consumable)
                    return GameManager.Instance.ConsumableDatabase.GetItem(drop.IdentifiedItemName);
                else if (drop.Type == ItemType.Equipment)
                    return GameManager.Instance.GameEquipment.GetEquipment(drop.IdentifiedItemName);
                else if (drop.Type == ItemType.Item)
                    return GameManager.Instance.ItemDatabase.GetItem(drop.IdentifiedItemName);
            }
        }
        return null;
    }

}

[System.Serializable]
public class LootDrop
{
    [SerializeField] public string IdentifiedItemName;
    [SerializeField] public ItemType Type;
    [SerializeField] public int Weight;

    public LootDrop()
    {

    }

    public LootDrop(string itemName,ItemType type, int weight)
    {
        IdentifiedItemName = itemName;
        Type = type;
        Weight = weight;
    }
}

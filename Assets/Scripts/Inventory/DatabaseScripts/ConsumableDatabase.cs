using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class ConsumableDatabase : ScriptableObject
{
    [SerializeField] private List<ConsumableItem> DataList;

    public int COUNT
    {
        get
        {
            return DataList.Count;
        }
    }

    void OnEnable()
    {

        Debug.Log("The consumable database is being enabled....");
        if (DataList == null)
            DataList = new List<ConsumableItem>();
    }

    public void AddItem(ConsumableItem _item)
    {
        DataList.Add(_item);
    }

    public void RemoveItem(ConsumableItem _item)
    {
        DataList.Remove(_item);
    }

    public void RemoveItemAt(int index)
    {
        DataList.RemoveAt(index);
    }

    public ConsumableItem GetItem(string _name)
    {
        foreach (ConsumableItem item in DataList)
        {
            if (item.IdentifiedName.Equals(_name))
                return item;
        }

        Debug.LogWarning("Could not find consumable with name: " + _name);
        return null;
    }

    public ConsumableItem GetItem(ConsumableItem item)
    {
        return GetItem(item.ItemName);
    }

    public ConsumableItem GetItemAt(int index)
    {

        return DataList.ElementAt(index);
    }

    public void SortAlphabeticallyAtoZ()
    {
        DataList.Sort((x, y) => string.Compare(x.ItemName, y.ItemName));
    }
}

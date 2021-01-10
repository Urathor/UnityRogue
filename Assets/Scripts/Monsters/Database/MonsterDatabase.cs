using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;



[System.Serializable]
public class MonsterDatabase : ScriptableObject
{
    [SerializeField] private List<MonsterData> DataList;

    public int COUNT
    {
        get
        {
            return DataList.Count;
        }
    }

    void OnEnable()
    {

        Debug.Log("The monster database is being enabled....");
        if (DataList == null)
            DataList = new List<MonsterData>();
    }

    public void AddItem(MonsterData _item)
    {
        DataList.Add(_item);
    }

    public void RemoveItem(MonsterData _item)
    {
        DataList.Remove(_item);
    }

    public void RemoveItemAt(int index)
    {
        DataList.RemoveAt(index);
    }

    public MonsterData GetItem(string _name)
    {
        foreach (MonsterData item in DataList)
        {
            if (item.Name.Equals(_name))
                return item;
        }

        Debug.LogWarning("Could not find monster with name: " + _name);
        return null;
    }

    public MonsterData GetItem(MonsterData item)
    {
        return GetItem(item.Name);
    }

    public MonsterData GetItemAt(int index)
    {

        return DataList.ElementAt(index);
    }

    public void SortAlphabeticallyAtoZ()
    {
        DataList.Sort((x, y) => string.Compare(x.Name, y.Name));
    }

}

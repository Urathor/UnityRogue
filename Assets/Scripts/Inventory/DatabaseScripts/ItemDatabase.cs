using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;



[System.Serializable]
public class ItemDatabase : ScriptableObject
{
	[SerializeField]private List<Item> DataList;

	public int COUNT
	{
		get 
		{ 
			return DataList.Count;
		}
	}

	void OnEnable()
	{

		Debug.Log ("The item database is being enabled....");
		if( DataList == null )
			DataList = new List<Item>();
	}

	public void AddItem( Item _item )
	{
		DataList.Add( _item );
	}

	public void RemoveItem( Item _item )
	{
		DataList.Remove( _item );
	}

	public void RemoveItemAt( int index )
	{
		DataList.RemoveAt( index );
	}

	public Item GetItem(string _name )
	{
		foreach(Item item in DataList)
		{
			if (item.IdentifiedName.Equals (_name))
				return item;
		}

		Debug.LogWarning ("Could not find item with name: " + _name);
		return null;
	}

	public Item GetItem(Item item)
	{
		return GetItem (item.ItemName);
	}

	public Item GetItemAt(int index )
	{
			
		return DataList.ElementAt( index );
	}

	public void SortAlphabeticallyAtoZ()
	{
		DataList.Sort((x, y) => string.Compare(x.ItemName, y.ItemName));
	}

}

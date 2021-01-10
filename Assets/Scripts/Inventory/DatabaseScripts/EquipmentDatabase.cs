using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class EquipmentDatabase : ScriptableObject
{
	[SerializeField]private  List<EquipmentItem> DataList;

	public int COUNT 
	{
		get
		{ 
			return DataList.Count;
		}
	}

	void OnEnable()
	{

		Debug.Log ("The equipment database is being enabled....");
		if( DataList == null )
			DataList = new List<EquipmentItem>();
	}

	public void AddEquipment( EquipmentItem _item )
	{
		DataList.Add( _item );
	}

	public void RemoveEquipment(EquipmentItem _item )
	{
		DataList.Remove( _item );
	}

	public void RemoveAt( int index )
	{
		DataList.RemoveAt( index );
	}
			
	public EquipmentItem GetEquipment(string _name )
	{
		foreach(EquipmentItem item in DataList)
		{
			if (item.IdentifiedName.Equals (_name))
				return item;
		}

		Debug.LogWarning ("Could not find equipment with name: " + _name);
		return null;
	}

	public EquipmentItem GetEquipment(EquipmentItem item)
	{
		return GetEquipment (item.ItemName);
	}

	public EquipmentItem GetEquipmentAt(int index )
	{

		return DataList.ElementAt( index );
	}

	public void SortAlphabeticallyAtoZ()
	{
		DataList.Sort((x, y) => string.Compare(x.ItemName, y.ItemName));
	}

}

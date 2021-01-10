using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;



[System.Serializable]
public class ModifierDatabase : ScriptableObject
{
	[SerializeField]private List<StatModifier> DataList;

	public int COUNT
	{
		get 
		{ 
			return DataList.Count;
		}
	}

	void OnEnable()
	{

		Debug.Log ("The Modifier database is being enabled....");
		if( DataList == null )
			DataList = new List<StatModifier>();
	}

	public void AddModifier( StatModifier _item )
	{
		DataList.Add( _item );
	}

	public void RemoveModifier( StatModifier _item )
	{
		DataList.Remove( _item );
	}

	public void RemoveModifierAt( int index )
	{
		DataList.RemoveAt( index );
	}

	public StatModifier GetModifier(string _name )
	{
		foreach(StatModifier item in DataList)
		{
			if (item.name.Equals (_name))
				return item;
		}

		Debug.LogWarning ("Could not find Stat with name: " + _name);
		return null;
	}

	public StatModifier GetModifier(StatModifier item)
	{
		return GetModifier (item.name);
	}

	public StatModifier GetModifierAt(int index )
	{

		return DataList.ElementAt( index );
	}

	public StatModifier GetCopyOfModifierAt(int index)
	{
		return new StatModifier (GetModifierAt (index).name,GetModifierAt (index).ModifiedStat,GetModifierAt (index).Value,GetModifierAt (index).Type,null);
	}
	public void SortAlphabeticallyAtoZ()
	{
		DataList.Sort((x, y) => string.Compare(x.name, y.name));
	}

}

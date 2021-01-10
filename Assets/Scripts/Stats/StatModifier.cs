using UnityEngine;
using System.Xml.Serialization;
using System;


public enum ModifierType
{
    Flat = 100,
    PercentMulti = 200,
    PercentAdd = 300
}

[Serializable]
[XmlInclude(typeof(EquipmentItem))]
public class StatModifier
{
	public string name;
	public BaseStat ModifiedStat;
    public  float Value;
    public  ModifierType Type;
	public string Source;
	private  int m_order;

	public int Order 
	{
		get
		{
			m_order = (int)this.Type;
			return m_order;
		}
	}

	void OnValidate()
	{
		m_order = (int)this.Type;
	}

	public StatModifier(string _name, BaseStat stat, float value, ModifierType type, string src)
	{
		name = _name;
		ModifiedStat = stat;
		Value = value;
		Type = type;
		Source = src;
	}
}

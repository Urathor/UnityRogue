using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;


[Serializable] [CreateAssetMenu(menuName = "Stats/BaseStat")]
public class BaseStat : ScriptableObject
{
    //public variables
    public string DisplayName;
    public int BaseValue;
        
	[SerializeField]public readonly ReadOnlyCollection<StatModifier> StatModifiers;

	//protected variables
	[SerializeField]protected readonly List<StatModifier> m_statModifiers;
    protected bool m_isDirty = true;
    protected int m_value;
    protected int m_lastBaseValue = int.MinValue;

    public virtual int FinalValue
    {
        get
        {
            if (m_isDirty || BaseValue != m_lastBaseValue)
            {
                m_lastBaseValue = BaseValue;

                m_value = CalculateFinalValue();
                m_isDirty = false;
            }

            return m_value;

        }
    }

    public BaseStat()
    {
        m_statModifiers = new List<StatModifier>();
        StatModifiers = m_statModifiers.AsReadOnly();

        m_isDirty = true;

    }

    public BaseStat(string strName, int val) : this()
    {
        name = strName;
        BaseValue = val;
    }

    protected virtual int CompareModifierOrder(StatModifier a, StatModifier b)
    {
        if (a.Order < b.Order)
        {
            return -1;
        }
        else if (a.Order > b.Order)
        {
            return 1;
        }

        return 0;
    }

    protected virtual int CalculateFinalValue()
    {
        float finalValue = BaseValue;
        float sumPercentAdd = 0f;

		for (int i = 0; i < m_statModifiers.Count; i++)
		{
			StatModifier mod = m_statModifiers [i];

			if (mod.Type == ModifierType.Flat)
			{
				finalValue += mod.Value;
			} 
			else if (mod.Type == ModifierType.PercentMulti)
			{
				finalValue *= 1 + mod.Value;
			} 
			else if (mod.Type == ModifierType.PercentAdd)
			{
				sumPercentAdd += mod.Value;

				if (i + 1 >= m_statModifiers.Count || m_statModifiers [i + 1].Type != ModifierType.PercentAdd)
				{
					finalValue *= 1 + sumPercentAdd;
					sumPercentAdd = 0;
				}

			}
		}

        return (int)Math.Round(finalValue, 1);
    }

    public virtual void AddModifier(StatModifier mod)
    {
        m_isDirty = true;
        m_statModifiers.Add(mod);
        m_statModifiers.Sort(CompareModifierOrder);

    }

    public virtual bool RemoveModifier(StatModifier mod)
    {

        if (m_statModifiers.Remove(mod))
        {
            m_isDirty = true;
            return true;
        }
        return false;
    }
        
	public virtual bool RemoveModifiersFromSource(string source)
    {
        bool didRemove = false;

        for (int i = m_statModifiers.Count - 1; i >= 0; i--)
        {
            if (m_statModifiers[i].Source == source)
            {
                didRemove = true;
                m_isDirty = true;
                m_statModifiers.RemoveAt(i);
            }
        }

        return didRemove;
    }

    public virtual void RemoveAllModifiers()
    {
        m_statModifiers.Clear();
        m_isDirty = true;
    }

}


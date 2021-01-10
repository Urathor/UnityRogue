using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EquipmentType
{
    Weapon,
    Armor,
    Ring,
    Amulet,

}
[System.Serializable]
public class EquipmentItem : Item
{

    [SerializeField] public EquipmentType EquipmentType;
    [SerializeField] private List<StatModifier> m_statMods;
    [HideInInspector] public int[] statIndex;

    public List<StatModifier> StatModifiers
    {
        get
        {
            return m_statMods;
        }
        set
        {
            m_statMods = value;
        }


    }
    public EquipmentItem()
    {

    }
    public EquipmentItem(string name, Sprite icon, EquipmentType type, List<StatModifier> mods)
    {

        ItemName = name;
        ItemIcon = icon;
        EquipmentType = type;
        m_statMods = mods;
    }
    public void Equip(Player c)
    {
        for (int i = 0; i < m_statMods.Count; i++)
        {
            foreach (BaseStat stat in c.Stats)
            {
                if (stat == m_statMods[i].ModifiedStat)
                {
                    stat.AddModifier(m_statMods[i]);
                }
            }
        }

        UIEventHandler.StatsChanged();
    }

    public void Unequip(Player c)
    {

        foreach (BaseStat stat in c.Stats)
        {
            stat.RemoveModifiersFromSource(this.ItemName);
        }

        UIEventHandler.StatsChanged();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentPanel : MonoBehaviour
{
    public Transform m_equipmentParent;
    [SerializeField] private EquipmentDisplayItem[] m_equipmentSlots;

    private void Awake()
    {
        if (m_equipmentParent != null)
            m_equipmentSlots = m_equipmentParent.GetComponentsInChildren<EquipmentDisplayItem>();

        foreach (EquipmentDisplayItem eqSlot in m_equipmentSlots)
        {
            eqSlot.Init();
        }
    }


    public bool AddItem(EquipmentItem item)
    {
        for (int i = 0; i < m_equipmentSlots.Length; i++)
        {
            if (m_equipmentSlots[i].Type == item.EquipmentType)
            {
                m_equipmentSlots[i].SetItem(item);
                return true;
            }
        }

        return false;
    }

    public bool RemoveItem(EquipmentItem item)
    {
        for (int i = 0; i < m_equipmentSlots.Length; i++)
        {
            if (m_equipmentSlots[i].Type == item.EquipmentType)
            {
                m_equipmentSlots[i].SetEmpty();

                return true;
            }
        }

        return false;
    }
}

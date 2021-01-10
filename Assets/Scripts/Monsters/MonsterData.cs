using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MonsterData
{
    [SerializeField] public Sprite Icon;
    [SerializeField] public string Name;
    [SerializeField] public int Level;
    [SerializeField] public string MaxHealth;
    [SerializeField] public int Attacks;
    [SerializeField] public int HitBonus;
    [SerializeField] public string Damage;
    [SerializeField] public string Defense;
    [SerializeField] public int DamageBonus;
    [SerializeField] public int Speed;
    [SerializeField] public string Awareness;
    [SerializeField] public string GoldDrop;
    [SerializeField] public string ExpValue;
    [SerializeField] public Color Color;
    [SerializeField] public bool CanFlee;
    [SerializeField] public bool Stationary;
    [SerializeField] public bool CanOpenDoors;
    [SerializeField] public Condition SpecialAttack;
    [SerializeField] public int ChanceForLoot;
    [SerializeField] public List<LootDrop> Drops;
    
    public MonsterData()
    {

    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueSharp;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(SpriteRenderer))]
public class Actor : Entity, IScheduleable
{
    public string DisplayName { get; set; }
    public int Level { get; set; }
    public int Gold { get; set; }
    public int CurrentHealth { get; set; }
    
    public int Strength { get { return strength.FinalValue; } }
    public int MaxHealth { get { return maxHealth.FinalValue; } }
    public int Attack { get { return attack.FinalValue; } }
    public int Damage { get { return damage.FinalValue; } }
    public int Defense { get { return defense.FinalValue; } }
    public int HitBonus { get { return hitBonus.FinalValue; } }
    public int DamageBonus { get { return damageBonus.FinalValue; } }
    public int Awareness { get { return awareness.FinalValue; } }
    public int Time { get { return speed.FinalValue; } }
    public bool CanOpenDoors { get; set; }
    public bool CanMove { get; set; }
    //Stat Properties
    public BaseStat strength;
    public BaseStat maxHealth;
    public BaseStat attack;
    public BaseStat hitBonus;
    public BaseStat defense;
    public BaseStat damageBonus;
    public BaseStat awareness;
    public BaseStat speed;
    public BaseStat damage;
    public BaseStat capacity;

    public virtual void Init()
    {
        //nothing in the base
    }
    public virtual void OnDeath()
    {
        //nothing in the base
    }
    public void Draw( IMap map)
    {

        // if this cell has not been explored don't render it
        if(!map.IsExplored(X,Y))
        {
            if (go.GetComponent<SpriteRenderer>().enabled)
                go.GetComponent<SpriteRenderer>().enabled = false;

            return;
        }

        //if this cell is in the FoV
        if(map.IsInFov(X,Y))
        {
            if (go == null)
                Debug.Log($"{this.DisplayName} did not have a gameObject tied to it.");
            else
            {
                go.GetComponent<Renderer>().enabled = true;
            }
        }
        else
        {
            if (go == null)
                Debug.Log($"{this.DisplayName} did not have a gameObject tied to it.");
            else
                go.GetComponent<Renderer>().enabled = false;
        }
    }
}

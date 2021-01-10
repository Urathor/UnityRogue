using System;
using UnityEngine;
using Cinemachine;
using RogueSharp.DiceNotation;
using System.Collections.Generic;

public class Player : Actor
{
    public int Experience;
    public int NeededExperience;
    public int HealthRegenTime;
    public int HungerReductionTime;
    public List<BaseStat> Stats;
    public List<Condition> PlayerConditions;
    public float Hunger { get { return hunger; } }
    private int timeTilRegenTick;
    private int timeTilHungerTick;
    private float hunger = 100f;
    private int starvationRate = 1;

    //Equipment Slots
    private EquipmentItem currentWeapon;
    private EquipmentItem currentArmor;
    private EquipmentItem currentRing1;
    private EquipmentItem currentRing2;
    private EquipmentItem currentAmulet;

    #region Core Functions
    public override void Init()
    {
        PlayerConditions = new List<Condition>();

        DisplayName = "Rogue";
        X = 10;
        Y = 10;

        strength.BaseValue = 16;
        maxHealth.BaseValue = 15;
        attack.BaseValue = 1;

        if (Strength == 17 || Strength == 18)
            hitBonus.BaseValue = 1;
        else if (Strength == 19 || Strength == 20)
            hitBonus.BaseValue = 2;
        else if (Strength > 20 || Strength <= 30)
            hitBonus.BaseValue = 3;
        else if (Strength == 31)
            hitBonus.BaseValue = 4;


        defense.BaseValue = 0;
        //NumberOfBlocks = 1 + Convert.ToInt32(Level / 3);
        //blockChance.BaseValue = 30;
        awareness.BaseValue = 3;
        speed.BaseValue = 10;
        CanOpenDoors = true;
        CanMove = true;

        gameObject.transform.position = new Vector2(X, Y);
        this.go = this.gameObject;
        GameObject.Find("CameraMan1").GetComponent<CinemachineVirtualCamera>().Follow = this.go.transform;

        CurrentHealth = maxHealth.FinalValue;
        Experience = 0;
        NeededExperience = 100;
        Level = 1;

        Stats = new List<BaseStat>() { strength ,attack, hitBonus, defense, speed, maxHealth, awareness,damage, capacity };

        foreach (BaseStat stat in Stats)
        {
            stat.RemoveAllModifiers();
        }
        timeTilRegenTick = 21-(GameManager.MapLevel*2);
        timeTilHungerTick = HungerReductionTime;

        UIEventHandler.StatsChanged();
    }

    public void GiveGold(int amt)
    {
        Gold += amt;

        UIEventHandler.StatsChanged();
    }

    public void GrantExp(int amt)
    {
        Experience += amt;

        GameManager.MessageLog.AddLog($"{DisplayName} gained {amt} experience.");

        while (Experience > NeededExperience)
        {
            Experience = Experience - NeededExperience;
            
            LevelUp();
        }

        UIEventHandler.StatsChanged();
    }

    public void LevelUp()
    {
        Level++;
        maxHealth.BaseValue += Dice.Roll("1D10");

        CurrentHealth = MaxHealth;
        NeededExperience = Convert.ToInt32( NeededExperience * 1.25);

        UIEventHandler.StatsChanged();
        GameManager.MessageLog.AddLog($"{this.DisplayName} has gained a level and become more powerful!!");

    }

    public void Heal(int amt)
    {
        CurrentHealth += amt;

        if (CurrentHealth > MaxHealth)
            CurrentHealth = MaxHealth;

        UIEventHandler.StatsChanged();
    }

    public void TakeDamage(int amount)
    {
        CurrentHealth -= amount;

        if(CurrentHealth < 1)
        {
            OnDeath();
        }

        UIEventHandler.StatsChanged();
    }

    public void ChangeHunger(float amt)
    {
        hunger += amt;

        if (hunger > 100)
            hunger = 100;

        if (hunger < 1)
        {
            hunger = 0;
            this.CurrentHealth -= starvationRate;
            GameManager.MessageLog.AddLog($"{this.DisplayName} is starving and loses {starvationRate} life.");
        }

        UIEventHandler.StatsChanged();
    }

    public void Regenerate()
    {
        timeTilRegenTick--;
        
        if (timeTilRegenTick < 1)
        {
            if (GameManager.MapLevel < 8)
            {
                Heal(1);
                timeTilRegenTick = 21 - (GameManager.MapLevel * 2);
            }
            else
            {
                Heal(UnityEngine.Random.Range(1, GameManager.MapLevel - 7));
                timeTilRegenTick = 3;
            }
        }
    }

    public void ReduceHunger()
    {
        timeTilHungerTick--;

        if(timeTilHungerTick <1)
        {
            ChangeHunger(-.75f);
            timeTilHungerTick = HungerReductionTime;
        }
    }

    public void Consume(ConsumableItem item)
    {
        item.OnConsume();
    }

    public override void OnDeath()
    {
        //EndGame
    }

    #endregion

    #region Equipment Functions

    public void EquipItem(EquipmentItem itemToEquip)
    {
        switch (itemToEquip.EquipmentType)
        {
            case EquipmentType.Weapon: EquipWeapon(itemToEquip); break;
            case EquipmentType.Armor: EquipArmor(itemToEquip); break;
        }
    }

    public void UnequipItem(EquipmentItem itemToRemove)
    {
        switch (itemToRemove.EquipmentType)
        {
            case EquipmentType.Weapon: UnequipWeapon(itemToRemove); break;
            case EquipmentType.Armor: UnequipArmor(itemToRemove); break;
        }
    }

    private void EquipWeapon(EquipmentItem itemToEquip)
    {
        if (currentWeapon != null)
        {
            UnequipWeapon(currentWeapon);
        }

        currentWeapon = itemToEquip;
        currentWeapon.Equip(this);
        GameManager.Instance.EquipmentPanel.AddItem(currentWeapon);
        UIEventHandler.ItemEquipped(currentWeapon);
        UIEventHandler.StatsChanged();
    }

    private void UnequipWeapon(EquipmentItem itemToRemove)
    {
        itemToRemove.Unequip(this);
        GameManager.Instance.EquipmentPanel.RemoveItem(itemToRemove);
        UIEventHandler.StatsChanged();
        GameManager.InventorySystem.AddItem(itemToRemove);
        currentWeapon = null;
    }

    private void EquipArmor(EquipmentItem itemToEquip)
    {
        if (currentArmor != null)
        {
            UnequipWeapon(currentArmor);
        }

        currentArmor = itemToEquip;
        currentArmor.Equip(this);
        GameManager.Instance.EquipmentPanel.AddItem(currentArmor);
        UIEventHandler.ItemEquipped(currentArmor);
        UIEventHandler.StatsChanged();
    }

    private void UnequipArmor(EquipmentItem itemToRemove)
    {
        itemToRemove.Unequip(this);
        GameManager.Instance.EquipmentPanel.RemoveItem(itemToRemove);
        UIEventHandler.StatsChanged();
        GameManager.InventorySystem.AddItem(itemToRemove);
        currentArmor = null;
    }

    #endregion
}

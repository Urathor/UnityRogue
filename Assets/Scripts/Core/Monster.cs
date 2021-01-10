using System;
using UnityEngine;
using RogueSharp.DiceNotation;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
public class Monster : Actor
{
   
    public int ExpValue;
    public int? TurnsAlerted { get; set; }
    public Color Color { get; set; }
    public LootGenerator LootTable { get; set; }
    public MonsterData Data;


    public override void Init()
    {
        this.GetComponent<SpriteRenderer>().sprite = Data.Icon;
        this.GetComponent<SpriteRenderer>().enabled = false;
        this.Level = Data.Level;
        this.DisplayName = Data.Name;
        this.maxHealth = Res.Instance.CreateStat("Max Health", Dice.Roll(Data.MaxHealth));
        this.attack = Res.Instance.CreateStat("Attack", Data.Attacks);
        this.hitBonus = Res.Instance.CreateStat("Hit Bonus", Data.HitBonus);
        this.defense = Res.Instance.CreateStat("Defense", Dice.Roll(Data.Defense));
        this.damageBonus = Res.Instance.CreateStat("Block Chance", Data.DamageBonus);
        this.damage = Res.Instance.CreateStat("Damage", Dice.Roll(Data.Damage));
        this.speed = Res.Instance.CreateStat("Speed", Data.Speed);
        this.awareness = Res.Instance.CreateStat("Awareness", Dice.Roll(Data.Awareness));
        this.Gold = Dice.Roll(Data.GoldDrop);
        this.ExpValue = Dice.Roll(Data.ExpValue);
        this.Color = Data.Color;
        this.CanOpenDoors = Data.CanOpenDoors;
        this.CanMove = true;
        this.CurrentHealth = this.maxHealth.FinalValue;

        LootTable = new LootGenerator();
        LootTable.LootList = Data.Drops;

    }

    public virtual void PerformAction(CommandSystem commandSystem)
    {
        IBehavior behavior;

        if(Data.Stationary)
        {
            behavior = new StationaryAttack();
        }
        else if (!Data.CanFlee)
        {
            behavior = new StandardAttackAndMove();
        }
        else
        {
            if (this.CurrentHealth < Convert.ToInt32(this.MaxHealth / 2))
                behavior = new FleeFromPlayer();
            else
                behavior = new StandardAttackAndMove();
        }

        behavior.Act(this, commandSystem);
    }

    public void DrawStats()
    {
        EnemyPanel.Instance.AddMonsterToList(this);
    }

    public virtual void DropLoot()
    {
        Item item = LootTable.GetDrop();

        if(item != null)
        {
            GameObject lootObject = Instantiate(Res.Instance.Loot, new Vector2(this.X, this.Y), Quaternion.identity);
            Loot loot = lootObject.GetComponent<Loot>();
            loot.X = this.X;
            loot.Y = this.Y;
            loot.go = lootObject;
            lootObject.transform.SetParent(GameManager.ObjectHolder);
            loot.ItemName = item.IdentifiedName;
            loot.Type = item.Type;
            loot.Renderer.sprite = item.ItemIcon;
            GameManager.DungeonMap.AddLoot(loot);
        }
    }

    public override void OnDeath()
    {
        int roll = UnityEngine.Random.Range(1, 101);

        if(roll < Data.ChanceForLoot)
            DropLoot();
    }
}

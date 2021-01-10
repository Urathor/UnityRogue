using RogueSharp.DiceNotation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flames : Effect
{
    public string Damage = "2D6";

    public override void onTick()
    {
        GameManager.Player.TakeDamage(Dice.Roll(Damage));
    }
}

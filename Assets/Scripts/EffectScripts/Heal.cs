using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/Heal Effect")]
public class Heal : Effect
{
    public int HealAmountMin;
    public int HealAmountMax;

    public override void onTick()
    {
        int healAmount = Random.Range(HealAmountMin, HealAmountMax) + 1;
        GameManager.MessageLog.AddLog(Description);
        GameManager.Player.Heal(healAmount);
    }
}

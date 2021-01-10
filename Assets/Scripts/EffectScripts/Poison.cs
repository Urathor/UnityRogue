using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/Poison Effect")]
public class Poison : Effect
{
    public int PoisonAmount;

    public override void onTick()
    {
        GameManager.Player.TakeDamage(PoisonAmount);
        GameManager.MessageLog.AddLog(Description);
    }
}

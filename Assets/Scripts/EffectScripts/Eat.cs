using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/Eat Effect")]
public class Eat : Effect
{
    public float HungerGain;

    public override void onTick()
    {
        GameManager.Player.ChangeHunger(HungerGain);
        GameManager.MessageLog.AddLog(Description);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/Hold Player Effect")]
public class HoldPlayer : Effect
{
    public override void onTick()
    {
        GameManager.Player.CanMove = false;
        GameManager.MessageLog.AddLog(Description);
    }
}
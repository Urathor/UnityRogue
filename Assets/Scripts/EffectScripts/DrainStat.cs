using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/Drain Stat Effect")]
public class DrainStat : Effect
{
    public int DrainAmount;
    public string DrainedStat;

    public override void onTick()
    {
        Player player = GameManager.Player;
        
        foreach(BaseStat stat in player.Stats)
        {
            if (stat.name == DrainedStat)
            {
                stat.BaseValue -= DrainAmount;
                if (stat.BaseValue < 1)
                    stat.BaseValue = 1;
            }
            else
                continue;
        }
        GameManager.MessageLog.AddLog(Description);
    }
}

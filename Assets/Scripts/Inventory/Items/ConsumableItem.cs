using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConsumableItem : Item
{
    public Condition AppliedCondition;
    public bool ImediateEffect = false;


    public ConsumableItem()
    {

    }
    public ConsumableItem(string name, Sprite icon, bool stackable = true)
    {

        ItemName = name;
        ItemIcon = icon;
        Stackable = stackable;
    }

    public void OnConsume()
    {
        int counter = 0;
        foreach (Effect effect in AppliedCondition.Effects)
        {
            if (effect.ImmediateEffect)
            {
                effect.onTick();
            }
            else
            {
                Condition condition = new Condition();
                condition.Effects = AppliedCondition.Effects;
                condition.Duration = AppliedCondition.Duration;
                condition.Time = AppliedCondition.Time;

                GameManager.SchedulingSystem.Add(condition);
            }

            counter++;
        }
        
    }
}

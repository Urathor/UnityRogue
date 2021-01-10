using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Condition")]
public class Condition : ScriptableObject, IScheduleable
{
    public string DisplayName;
    public int Duration;
    public int Time { get { return time; } set { time = value; } }
    public List<Effect> Effects;
    public bool AlwaysOn = false;
    [SerializeField]private int time = 100;
    

    public void PerformAction(CommandSystem commandSystem)
    {

        foreach(Effect effect in Effects)
        {
            effect.onTick();
            if (effect.ImmediateEffect)
                Effects.Remove(effect);
        }

        if (AlwaysOn)
            return;

        if (Duration > 0 && Duration < 99)
            Duration--;

    }
}

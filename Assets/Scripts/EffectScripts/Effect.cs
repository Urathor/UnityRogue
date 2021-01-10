using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Effects/EmptyEffect")]
public class Effect : ScriptableObject, IEffect
{
    public bool ImmediateEffect = false;
    public string Description;
    public virtual void onTick()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/Hunger Effect")]
public class Hunger : Effect
{
    public float hungerRate = -1f;

    public override void onTick()
    {
        GameManager.Player.ChangeHunger(hungerRate);
        Debug.Log("Hunger grows....");
    }
}

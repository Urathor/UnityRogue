using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class StatPanel : MonoBehaviour
{
    public StatDisplay DungeonLvlDisplay;
    public StatDisplay HealthDisplay;
    public StatDisplay AttackDisplay;
    public StatDisplay DefenseDisplay;
    public StatDisplay ExperienceDisplay;
    public StatDisplay GoldDisplay;
    public StatDisplay HungerDisplay;

    // Start is called before the first frame update
    void Start()
    {
        UIEventHandler.OnStatsChanged += UpdateStatValues;
        
    }

    // Update is called once per frame
    void UpdateStatValues()
    {

        DungeonLvlDisplay.StatText.text = GameManager.MapLevel.ToString();

        int hp = Convert.ToInt32(((float)GameManager.Player.CurrentHealth / (float)GameManager.Player.MaxHealth) * 100f);
        int xp = Convert.ToInt32(((float)GameManager.Player.Experience / (float)GameManager.Player.NeededExperience) * 100f);
        int fp = Convert.ToInt32(((float)GameManager.Player.Hunger));
        HealthDisplay.StatText.text = $"{hp}%";
        AttackDisplay.StatText.text = $"{GameManager.Player.Strength}";
        DefenseDisplay.StatText.text = $"{GameManager.Player.Defense}";
        ExperienceDisplay.StatText.text = $"{GameManager.Player.Level} ({xp}%)";
        GoldDisplay.StatText.text = GameManager.Player.Gold.ToString();
        HungerDisplay.StatText.text = $"{fp}%";
    }
}

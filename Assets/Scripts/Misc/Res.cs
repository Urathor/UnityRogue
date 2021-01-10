using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Res : MonoBehaviour
{
    public const string GameElementPath = "GameElements/";
    public static Res Instance;
    public GameObject PlayerPrefab;
    public GameObject EnemyStatDisplay;
    public GameObject MessageLogText;
    public GameObject DoorObject;
    public GameObject StairsObject;
    public GameObject Monster;
    public GameObject Loot;


    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance.gameObject);

        Instance = this;
    }


    public BaseStat CreateStat(string name, int value)
    {
        BaseStat stat = ScriptableObject.CreateInstance<BaseStat>();

        stat.DisplayName = name;
        stat.BaseValue = value;

        return stat;
    }
}
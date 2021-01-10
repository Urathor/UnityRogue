using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogueSharp;
using System;
using RogueSharp.Random;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static DungeonMap DungeonMap { get; private set; }
    public static CommandSystem CommandSystem { get; private set; }
    public static MessageLog MessageLog { get; set; }
    public static SchedulingSystem SchedulingSystem { get; private set; }
    public static Player Player { get; set; }
    public static Dictionary<string, GameObject> Objects { get; set; }
    public static IRandom Random { get; private set; }
    public static InventorySystem InventorySystem { get; private set; }
    public static Tilemap BoardHolder { get; set; }
    public static Transform ObjectHolder { get; set; }
    public static int MapLevel = 1;

    [Header("Game Management")]
    public Transform TextContent;
    public ScrollRect scrollView;
    public EquipmentDatabase GameEquipment;
    public ItemDatabase ItemDatabase;
    public ConsumableDatabase ConsumableDatabase;
    public MonsterDatabase MonsterDatabase;
    public float KeyPressDelay = .2f;

    [Header("Map Settings")]
    public int LVL_WIDTH = 50;
    public int LVL_HEIGHT = 50;
    public RandomTile floor;
    public RuleTile wall;
    public Tilemap tilemap;
    public int seed { get; set; }

    [Header("UI Management")]
    public ItemDetailsObject ItemDetailsPanel;
    public EquipmentPanel EquipmentPanel;
    public GameObject InventoryFrame;


    private float lastKeyPressTime;
    private bool renderRequired = true;


    private bool ShowInventory = false;

    void InitGameSystems()
    {
        seed = (int)DateTime.Now.Ticks;
        Random = new DotNetRandom(seed);
        BoardHolder = tilemap;

        ObjectHolder = GameObject.Find("Object Holder").transform;
        
        CommandSystem = new CommandSystem();
        MessageLog = new MessageLog();
        SchedulingSystem = new SchedulingSystem();
       
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != null)
            Destroy(Instance.gameObject);
        Instance = this;

        Screen.SetResolution(1600, 900, false);

        InitGameSystems();

        MapGenerator mapGen = new MapGenerator(LVL_WIDTH,LVL_HEIGHT, 20 ,10,6, MapLevel);
        DungeonMap = mapGen.CreateMap();
        DungeonMap.UpdatePlayerFoV();

        InventorySystem = new InventorySystem();

        InventorySystem.EquipItem(GameEquipment.GetEquipment("Simple Mace"));
        InventorySystem.EquipItem(GameEquipment.GetEquipment("Plain Ringmail"));
        InventorySystem.AddMultiItems("Simple Bread", ItemType.Consumable,2);

        InventoryFrame.SetActive(ShowInventory);

        // Create a new MessageLog and print the random seed used to generate the level
        MessageLog.AddLog("The rogue arrives on level 1");
        MessageLog.AddLog($"Level {MapLevel} created with seed '{seed}'");

    }
    private void LateUpdate()
    {
        if(renderRequired)
        {
            if (EnemyPanel.Instance != null)
            {
                EnemyPanel.Instance.ClearList();

                for (int i = 0; i < EnemyPanel.Instance.ContentParent.childCount; i++)
                {
                    var go = EnemyPanel.Instance.ContentParent.GetChild(i).gameObject;
                    Destroy(go);
                }
            }

            DungeonMap.Draw(tilemap);
            MessageLog.Draw(TextContent, scrollView);

            UIEventHandler.StatsChanged();
            renderRequired = false;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        bool didPlayerAct = false;

        if (CommandSystem.IsPlayerTurn)
        {
            if (Input.anyKey && Time.time - lastKeyPressTime > KeyPressDelay)
            {
                lastKeyPressTime = Time.time;

                if (Player.CanMove)
                {
                    if (Input.GetKey(KeyCode.UpArrow))
                        didPlayerAct = CommandSystem.MovePlayer(Direction.Up);
                    else if (Input.GetKey(KeyCode.DownArrow))
                        didPlayerAct = CommandSystem.MovePlayer(Direction.Down);
                    else if (Input.GetKey(KeyCode.RightArrow))
                        didPlayerAct = CommandSystem.MovePlayer(Direction.Right);
                    else if (Input.GetKey(KeyCode.LeftArrow))
                        didPlayerAct = CommandSystem.MovePlayer(Direction.Left);
                }

                if (Input.GetKey(KeyCode.Period))
                {
                    if (DungeonMap != null && DungeonMap.CanMoveDownStairs())
                    {
                        LoadNextLevel();
                    }
                }
                else if (Input.GetKey(KeyCode.I))
                {
                    ShowInventory = !ShowInventory;
                    InventoryFrame.SetActive(ShowInventory);
                }
                else if (Input.GetKey(KeyCode.G))
                {
                    if (GameManager.InventorySystem.PlayerItems.Count < GameManager.Player.capacity.FinalValue)
                    {
                        Loot loot = DungeonMap.GetLootAt(Player.X, Player.Y);

                        if (loot != null)
                            loot.Interact();
                    }
                    else
                    {
                        GameManager.MessageLog.AddLog("You can not carry anymore.");
                    }
                }

            }

            if (didPlayerAct)
            {
                EndPlayerTurn();
            }
        }
        else
        {
            CommandSystem.AdvanceSchedule();
            renderRequired = true;
        }
    }

    public void EndPlayerTurn()
    {
        renderRequired = true;
        CommandSystem.EndPlayerTurn();
    }

    public void LoadNextLevel()
    {
        MapGenerator mapGen = new MapGenerator(LVL_WIDTH, LVL_HEIGHT, 20, 10, 6, ++MapLevel);
        DungeonMap = mapGen.CreateMap();
        MessageLog.ClearLog();
        CommandSystem = new CommandSystem();
        UIEventHandler.StatsChanged();
    }

    public RandomTile Floor
    {
        get { return floor; }
    }

    public RuleTile Wall
    {
        get{ return wall; }
    }
}

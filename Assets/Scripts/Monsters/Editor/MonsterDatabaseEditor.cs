using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public class MonsterDatabaseEditor : EditorWindow
{
    private enum State
    {
        BLANK,
        EDIT,
        ADD
    }
    private const string DATABASE_PATH = @"Assets/Scripts/Resources/Databases/MonsterDatabase.asset";
    private State state;
    private int selectedItem;
    private MonsterDatabase monsters;
    private Vector2 _scrollPos;

    //MonsterData
    private Sprite newMonsterIcon;
    private string newMonsterName;
    private int newMonsterLevel;
    private string newMonsterMaxHealth;
    private int newMonsterAttacks;
    private string newMonsterDamage;
    private int newMonsterHitBonus;
    private string newMonsterDefense;
    private int newMonsterDamageBonus;
    private int newMonsterSpeed;
    private string newMonsterAwareness;
    private string newMonsterGoldAmount;
    private string newMonsterExpValue;
    private Color newMonsterColor;
    private bool newMonsterCanFlee;
    private bool newMonsterStationary;
    private bool newMonsterCanOpenDoors;
    private int newMonsterChanceForLoot;
    private List<LootDrop> newMonsterLootDrops;


    [MenuItem("ECG/Monster Editor")]
    public static void Init()
    {
        MonsterDatabaseEditor window = EditorWindow.GetWindow<MonsterDatabaseEditor>();
        window.minSize = new Vector2(1000, 400);
        window.Show();
    }

    void CreateDatabase()
    {
        monsters = ScriptableObject.CreateInstance<MonsterDatabase>();

        AssetDatabase.CreateAsset(monsters, DATABASE_PATH);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    void LoadDatabase()
    {
        monsters = (MonsterDatabase)AssetDatabase.LoadAssetAtPath(DATABASE_PATH, typeof(MonsterDatabase));

        if (monsters == null)
            CreateDatabase();
    }

    void OnEnable()
    {

        if (monsters == null)
            LoadDatabase();

        state = State.BLANK;
    }

    void DisplayAddMainArea()
    {
        newMonsterIcon = (Sprite)EditorGUILayout.ObjectField("Monster Icon:", newMonsterIcon, typeof(Sprite), false);
        newMonsterName = EditorGUILayout.TextField(new GUIContent("Name:"), newMonsterName);
        newMonsterLevel = EditorGUILayout.IntField(new GUIContent("Base Level:"), newMonsterLevel);
        newMonsterMaxHealth = EditorGUILayout.TextField(new GUIContent("(Notation) Max Health:"), newMonsterMaxHealth);
        newMonsterAttacks = EditorGUILayout.IntField(new GUIContent("Base # of Attacks:"), newMonsterAttacks);
        newMonsterHitBonus = EditorGUILayout.IntField(new GUIContent("Base Hit Bonus:"), newMonsterHitBonus);
        newMonsterDamage = EditorGUILayout.TextField(new GUIContent("(Notation)Damage:"), newMonsterDamage);
        newMonsterDefense = EditorGUILayout.TextField(new GUIContent("(Notation)Defense:"), newMonsterDefense);
        newMonsterDamageBonus = EditorGUILayout.IntField(new GUIContent("Damage Bonus:"), newMonsterDamageBonus);
        newMonsterSpeed = EditorGUILayout.IntField(new GUIContent("Speed:"), newMonsterSpeed);
        newMonsterAwareness = EditorGUILayout.TextField(new GUIContent("(Notation) Awareness:"), newMonsterAwareness);
        newMonsterGoldAmount = EditorGUILayout.TextField(new GUIContent("(Notation) Gold Dropped:"), newMonsterGoldAmount);
        newMonsterExpValue = EditorGUILayout.TextField(new GUIContent("(Notation) Exp Value:"), newMonsterExpValue);
        newMonsterChanceForLoot = EditorGUILayout.IntField(new GUIContent("Chance for Loot:"), newMonsterChanceForLoot);
        newMonsterStationary = EditorGUILayout.Toggle("Is Stationary?", newMonsterStationary);
        newMonsterCanOpenDoors = EditorGUILayout.Toggle("Can Open Doors?", newMonsterCanOpenDoors);
        newMonsterCanFlee = EditorGUILayout.Toggle("Can Flee?", newMonsterCanFlee);

        newMonsterColor = EditorGUILayout.ColorField(new GUIContent("Label Color:"), newMonsterColor);


        if (newMonsterLootDrops == null)
        {
            if (GUILayout.Button("Add Loot", GUILayout.Width(100)))
            {
                newMonsterLootDrops = new List<LootDrop>();
                LootDrop newDrop = new LootDrop();
                newMonsterLootDrops.Add(newDrop);
            }
        }
        else
        {
            for(int i = 0; i < newMonsterLootDrops.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                newMonsterLootDrops[i].IdentifiedItemName = EditorGUILayout.TextField(new GUIContent("Identified Item Name:"), newMonsterLootDrops[i].IdentifiedItemName);
                newMonsterLootDrops[i].Type = (ItemType)EditorGUILayout.EnumPopup("Item Type:", newMonsterLootDrops[i].Type);
                newMonsterLootDrops[i].Weight = EditorGUILayout.IntField(new GUIContent("Drop Weight:"), newMonsterLootDrops[i].Weight);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();

            if(GUILayout.Button("Add Loot", GUILayout.Width(100)))
            {
                LootDrop newDrop = new LootDrop();
                newMonsterLootDrops.Add(newDrop);
            }

            if (newMonsterLootDrops.Count > 0)
            {
                if (GUILayout.Button("Remove Loot", GUILayout.Width(100)))
                {
                    newMonsterLootDrops.RemoveAt(newMonsterLootDrops.Count - 1);
                }
            }
            EditorGUILayout.EndHorizontal();
        }


        if(GUILayout.Button("Done", GUILayout.Width(100)))
        {
            MonsterData newMonster = new MonsterData();
            newMonster.Icon = newMonsterIcon;
            newMonster.Name = newMonsterName;
            newMonster.Level = newMonsterLevel;
            newMonster.MaxHealth = newMonsterMaxHealth;
            newMonster.Attacks = newMonsterAttacks;
            newMonster.HitBonus = newMonsterHitBonus;
            newMonster.Damage = newMonsterDamage;
            newMonster.Defense = newMonsterDefense;
            newMonster.DamageBonus = newMonsterDamageBonus;
            newMonster.Speed = newMonsterSpeed;
            newMonster.Awareness = newMonsterAwareness;
            newMonster.GoldDrop = newMonsterGoldAmount;
            newMonster.ExpValue = newMonsterExpValue;
            newMonster.ChanceForLoot = newMonsterChanceForLoot;
            newMonster.Stationary = newMonsterStationary;
            newMonster.CanOpenDoors = newMonsterCanOpenDoors;
            newMonster.Color = newMonsterColor;
            newMonster.Drops = newMonsterLootDrops;
            newMonster.CanFlee = newMonsterCanFlee;
            monsters.AddItem(newMonster);

            newMonsterIcon = null;
            newMonsterName = "";
            newMonsterLevel = 1;
            newMonsterMaxHealth = "3D5";
            newMonsterAttacks = 1;
            newMonsterHitBonus = -1;
            newMonsterDamage = "1D8";
            newMonsterDefense = "1D6";
            newMonsterDamageBonus = 0;
            newMonsterSpeed = 10;
            newMonsterAwareness = "2D2";
            newMonsterGoldAmount = "1D8";
            newMonsterExpValue = "2D5";
            newMonsterColor = Color.white;
            newMonsterStationary = false;
            newMonsterCanOpenDoors = false;
            newMonsterCanFlee = true;
            newMonsterLootDrops = null;
            newMonsterChanceForLoot = 0;

            EditorUtility.SetDirty(monsters);
            state = State.BLANK;
        }
    }

    void DisplayEditMainArea()
    {
        monsters.GetItemAt(selectedItem).Icon = (Sprite)EditorGUILayout.ObjectField("Monster Icon:", monsters.GetItemAt(selectedItem).Icon, typeof(Sprite), false);
        monsters.GetItemAt(selectedItem).Name = EditorGUILayout.TextField(new GUIContent("Name:"), monsters.GetItemAt(selectedItem).Name);
        monsters.GetItemAt(selectedItem).Level = EditorGUILayout.IntField(new GUIContent("Base Level:"), monsters.GetItemAt(selectedItem).Level);
        monsters.GetItemAt(selectedItem).MaxHealth = EditorGUILayout.TextField(new GUIContent("(Notation)Max Health:"), monsters.GetItemAt(selectedItem).MaxHealth);
        monsters.GetItemAt(selectedItem).Attacks = EditorGUILayout.IntField(new GUIContent("Base # of Attacks:"), monsters.GetItemAt(selectedItem).Attacks);
        monsters.GetItemAt(selectedItem).HitBonus = EditorGUILayout.IntField(new GUIContent("Base Hit Bonus:"), monsters.GetItemAt(selectedItem).HitBonus);
        monsters.GetItemAt(selectedItem).Damage = EditorGUILayout.TextField(new GUIContent("(Notation)Damage:"), monsters.GetItemAt(selectedItem).Damage);
        monsters.GetItemAt(selectedItem).Defense = EditorGUILayout.TextField(new GUIContent("(Notation)Defense:"), monsters.GetItemAt(selectedItem).Defense);
        monsters.GetItemAt(selectedItem).DamageBonus = EditorGUILayout.IntField(new GUIContent("Damage Bonus:"), monsters.GetItemAt(selectedItem).DamageBonus);
        monsters.GetItemAt(selectedItem).Speed = EditorGUILayout.IntField(new GUIContent("Speed:"), monsters.GetItemAt(selectedItem).Speed);
        monsters.GetItemAt(selectedItem).Awareness = EditorGUILayout.TextField(new GUIContent("(Notation)Awareness:"), monsters.GetItemAt(selectedItem).Awareness);
        monsters.GetItemAt(selectedItem).GoldDrop = EditorGUILayout.TextField(new GUIContent("(Notation)Gold Dropped:"), monsters.GetItemAt(selectedItem).GoldDrop);
        monsters.GetItemAt(selectedItem).ExpValue = EditorGUILayout.TextField(new GUIContent("(Notation)Exp Value:"), monsters.GetItemAt(selectedItem).ExpValue);
        monsters.GetItemAt(selectedItem).ChanceForLoot = EditorGUILayout.IntField(new GUIContent("Chance for loot:"), monsters.GetItemAt(selectedItem).ChanceForLoot);
        monsters.GetItemAt(selectedItem).Color = EditorGUILayout.ColorField(new GUIContent("Label Color:"), monsters.GetItemAt(selectedItem).Color);
        monsters.GetItemAt(selectedItem).Stationary = EditorGUILayout.Toggle("Is Stationary?: ", monsters.GetItemAt(selectedItem).Stationary);
        monsters.GetItemAt(selectedItem).CanOpenDoors = EditorGUILayout.Toggle("Can Open Doors?: ", monsters.GetItemAt(selectedItem).CanOpenDoors);
        monsters.GetItemAt(selectedItem).CanFlee = EditorGUILayout.Toggle("Can Flee?: ", monsters.GetItemAt(selectedItem).CanFlee);


        if (monsters.GetItemAt(selectedItem).Drops == null)
        {
            if (GUILayout.Button("Add Loot", GUILayout.Width(100)))
            {
                monsters.GetItemAt(selectedItem).Drops = new List<LootDrop>();
                LootDrop newDrop = new LootDrop();
                monsters.GetItemAt(selectedItem).Drops.Add(newDrop);
            }
        }
        else
        {
            for (int i = 0; i < monsters.GetItemAt(selectedItem).Drops.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                monsters.GetItemAt(selectedItem).Drops[i].IdentifiedItemName = EditorGUILayout.TextField(new GUIContent("Identified Item Name:"), monsters.GetItemAt(selectedItem).Drops[i].IdentifiedItemName);
                monsters.GetItemAt(selectedItem).Drops[i].Type = (ItemType)EditorGUILayout.EnumPopup("Item Type:", monsters.GetItemAt(selectedItem).Drops[i].Type);
                monsters.GetItemAt(selectedItem).Drops[i].Weight = EditorGUILayout.IntField(new GUIContent("Drop Weight:"), monsters.GetItemAt(selectedItem).Drops[i].Weight);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Add Loot", GUILayout.Width(100)))
            {
                LootDrop newDrop = new LootDrop();
                monsters.GetItemAt(selectedItem).Drops.Add(newDrop);
            }

            if (monsters.GetItemAt(selectedItem).Drops.Count > 0)
            {
                if (GUILayout.Button("Remove Loot", GUILayout.Width(100)))
                {
                    monsters.GetItemAt(selectedItem).Drops.RemoveAt(monsters.GetItemAt(selectedItem).Drops.Count - 1);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Done", GUILayout.Width(100)))
        {
            monsters.SortAlphabeticallyAtoZ();
            EditorUtility.SetDirty(monsters);
            state = State.BLANK;
        }
    }

    void DisplayBlankMainArea()
    {
        newMonsterIcon = null;
        newMonsterName = "";
        newMonsterLevel = 1;
        newMonsterMaxHealth = "3D5";
        newMonsterAttacks = 1;
        newMonsterHitBonus = -1;
        newMonsterDamage = "1D8";
        newMonsterDefense = "1D6";
        newMonsterDamageBonus = 0;
        newMonsterSpeed = 10;
        newMonsterAwareness = "2D2";
        newMonsterGoldAmount = "1D8";
        newMonsterExpValue = "2D5";
        newMonsterColor = Color.white;
        newMonsterStationary = false;
        newMonsterCanOpenDoors = false;
        newMonsterCanFlee = true;
        newMonsterLootDrops = null;
        newMonsterChanceForLoot = 0;

        EditorGUILayout.LabelField(
            "There are 3 things that can be displayed here.\n" +
            "1) Item info for editing\n" +
            "2) Blank fields for adding a new Monster\n" +
            "3) Blank Area",
            GUILayout.ExpandHeight(true));
    }

    void DisplayMainArea()
    {
        EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
        EditorGUILayout.Space();

        switch (state)
        {
            case State.ADD:
                DisplayAddMainArea();
                break;
            case State.EDIT:
                DisplayEditMainArea();
                break;
            default:
                DisplayBlankMainArea();
                break;
        }

        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();
    }

    void DisplayListArea()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(250));
        EditorGUILayout.Space();

        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, "box", GUILayout.ExpandHeight(true));


        for (int cnt = 0; cnt < monsters.COUNT; cnt++)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("-", GUILayout.Width(25)))
            {
                monsters.RemoveItemAt(cnt);
                monsters.SortAlphabeticallyAtoZ();
                EditorUtility.SetDirty(monsters);
                state = State.BLANK;
                return;
            }

            if (GUILayout.Button(monsters.GetItemAt(cnt).Name, "box", GUILayout.ExpandWidth(true)))
            {
                selectedItem = cnt;
                state = State.EDIT;
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();

        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
        EditorGUILayout.LabelField("Monsters: " + monsters.COUNT, GUILayout.Width(100));

        if (GUILayout.Button("New Monster"))
        {

            state = State.ADD;
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        if (GUILayout.Button("Done"))
        {
            EditorUtility.SetDirty(monsters);
            this.Close();
        }
        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();
    }

    void OnGUI()
    {

        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
        DisplayListArea();
        DisplayMainArea();
        EditorGUILayout.EndHorizontal();
    }

    private T[] GetAtPath<T>(string path)
    {
        ArrayList al = new ArrayList();
        string[] fileEntries = Directory.GetFiles(Application.dataPath + "/" + path);
        foreach (string fileName in fileEntries)
        {
            int index = fileName.LastIndexOf("/");
            string localPath = "Assets/" + path;
            if (index > 0)
                localPath += fileName.Substring(index);

            object t = AssetDatabase.LoadAssetAtPath(localPath, typeof(T));

            if (t != null)
                al.Add(t);
        }

        T[] result = new T[al.Count];
        for (int i = 0; i < al.Count; i++)
        {
            result[i] = (T)al[i];
        }

        return result;

    }
}

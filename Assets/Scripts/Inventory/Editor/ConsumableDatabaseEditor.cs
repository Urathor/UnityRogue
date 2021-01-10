using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public class ConsumableDatabaseEditor : EditorWindow
{
    private enum State
    {
        BLANK,
        EDIT,
        ADD
    }

    private State state;

    private int selectedItem;

    private string newItemName;
    private string newItemIdentifiedName;
    private Sprite newItemIcon;
    private string newItemDescription;
    private int newItemGoldValue;
    private bool newItemIdentified;
    private bool newItemStackable;
    private Condition newItemCondition;
    private string newItemLog;

    private const string DATABASE_PATH = @"Assets/Scripts/Resources/Databases/ConsumableDatabase.asset";

    private ConsumableDatabase items;
    private Vector2 _scrollPos;

    [MenuItem("ECG/Item System/Editors/Consumable Editor")]
    public static void Init()
    {

        ConsumableDatabaseEditor window = EditorWindow.GetWindow<ConsumableDatabaseEditor>();
        window.minSize = new Vector2(800, 400);
        window.Show();
    }
    void OnEnable()
    {

        if (items == null)
            LoadDatabase();

        state = State.BLANK;
    }
    void OnGUI()
    {

        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
        DisplayListArea();
        DisplayMainArea();
        EditorGUILayout.EndHorizontal();
    }
    void LoadDatabase()
    {

        items = (ConsumableDatabase)AssetDatabase.LoadAssetAtPath(DATABASE_PATH, typeof(ConsumableDatabase));

        if (items == null)
            CreateDatabase();
    }
    void CreateDatabase()
    {

        items = ScriptableObject.CreateInstance<ConsumableDatabase>();

        AssetDatabase.CreateAsset(items, DATABASE_PATH);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    void DisplayListArea()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(250));
        EditorGUILayout.Space();

        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, "box", GUILayout.ExpandHeight(true));


        for (int cnt = 0; cnt < items.COUNT; cnt++)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("-", GUILayout.Width(25)))
            {
                items.RemoveItemAt(cnt);
                items.SortAlphabeticallyAtoZ();
                EditorUtility.SetDirty(items);
                state = State.BLANK;
                return;
            }

            if (GUILayout.Button(items.GetItemAt(cnt).IdentifiedName, "box", GUILayout.ExpandWidth(true)))
            {
                selectedItem = cnt;
                state = State.EDIT;
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();

        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
        EditorGUILayout.LabelField("Items: " + items.COUNT, GUILayout.Width(100));

        if (GUILayout.Button("New Item"))
        {
            newItemIcon = null;
            newItemCondition = null;
            newItemName = "New Item";
            newItemIdentifiedName = "New Identified Item";
            newItemDescription = "New Item Description";
            newItemGoldValue = 0;
            newItemStackable = false;
            state = State.ADD;
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        if (GUILayout.Button("Done"))
        {
            EditorUtility.SetDirty(items);
            this.Close();
        }
        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();
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
    void DisplayBlankMainArea()
    {

        EditorGUILayout.LabelField(
            "There are 3 things that can be displayed here.\n" +
            "1) Item info for editing\n" +
            "2) Blank fields for adding a new Item\n" +
            "3) Blank Area",
            GUILayout.ExpandHeight(true));
    }
    void DisplayEditMainArea()
    {
        //Sprites
        items.GetItemAt(selectedItem).ItemIcon = (Sprite)EditorGUILayout.ObjectField("Item Icon: ", items.GetItemAt(selectedItem).ItemIcon, typeof(Sprite), false);
        items.GetItemAt(selectedItem).AppliedCondition = (Condition)EditorGUILayout.ObjectField("Applied Contidtion:", items.GetItemAt(selectedItem).AppliedCondition, typeof(Condition), false);
        //Text Fields
        items.GetItemAt(selectedItem).ItemName = EditorGUILayout.TextField(new GUIContent("Name: "), items.GetItemAt(selectedItem).ItemName);
        items.GetItemAt(selectedItem).IdentifiedName = EditorGUILayout.TextField(new GUIContent("Identifed Name: "), items.GetItemAt(selectedItem).IdentifiedName);
        items.GetItemAt(selectedItem).Description = EditorGUILayout.TextField(new GUIContent("Description: "), items.GetItemAt(selectedItem).Description);
        items.GetItemAt(selectedItem).GoldValue = EditorGUILayout.IntField(new GUIContent("Gold Value: "), items.GetItemAt(selectedItem).GoldValue);
        //toggles
        items.GetItemAt(selectedItem).IsIdentified = EditorGUILayout.Toggle("Identifed", items.GetItemAt(selectedItem).IsIdentified);
        items.GetItemAt(selectedItem).Stackable = EditorGUILayout.Toggle("Stackable", items.GetItemAt(selectedItem).Stackable);

        EditorGUILayout.Space();

        if (GUILayout.Button("Done", GUILayout.Width(100)))
        {
            items.SortAlphabeticallyAtoZ();
            EditorUtility.SetDirty(items);
            state = State.BLANK;
        }
    }
    void DisplayAddMainArea()
    {
        //object fields
        newItemIcon = (Sprite)EditorGUILayout.ObjectField("Item Icon: ", newItemIcon, typeof(Sprite), false);
        newItemCondition = (Condition)EditorGUILayout.ObjectField("Item Condition: ", newItemCondition, typeof(Condition), false);
        //text fields
        newItemName = EditorGUILayout.TextField(new GUIContent("Name: "), newItemName);
        newItemIdentifiedName = EditorGUILayout.TextField(new GUIContent("Identified Name: "), newItemIdentifiedName);
        newItemDescription = EditorGUILayout.TextField(new GUIContent("Description: "), newItemDescription);
        newItemGoldValue = EditorGUILayout.IntField(new GUIContent("Gold Value: "), newItemGoldValue);
        //toggles
        newItemIdentified = EditorGUILayout.Toggle("IsIdentified", newItemIdentified);
        newItemStackable = EditorGUILayout.Toggle("Stackable", newItemStackable);



        if (GUILayout.Button("Done", GUILayout.Width(100)))
        {


            ConsumableItem newItem = new ConsumableItem();
            newItem.ItemIcon = newItemIcon;
            newItem.ItemName = newItemName;
            newItem.IdentifiedName = newItemIdentifiedName;
            newItem.Description = newItemDescription;
            newItem.GoldValue = newItemGoldValue;
            newItem.IsIdentified = newItemIdentified;
            newItem.Stackable = newItemStackable;
            newItem.AppliedCondition = newItemCondition;


            newItem.Type = ItemType.Consumable;
            items.AddItem(newItem);

            newItemIcon = null;
            newItemCondition = null;
            newItemName = "New Item";
            newItemIdentifiedName = "New Identified Item";
            newItemDescription = "New Item Description";
            newItemGoldValue = 0;
            newItemStackable = false;

            EditorUtility.SetDirty(items);
            state = State.BLANK;
        }
    }
}

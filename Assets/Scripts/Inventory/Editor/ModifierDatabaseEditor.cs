using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;


public class ModifierDatabaseEditor : EditorWindow 
{

	private enum State {
		BLANK,
		EDIT,
		ADD
	}

	private State state;

	private int selectedItem;
	private string newModName;
	private float newModValue;
	private ModifierType newModType;
	private BaseStat newModModifiedStat;

	private const string DATABASE_PATH = @"Assets/Scripts/Resources/Databases/ModifierDatabase.asset";

	private ModifierDatabase modifiers;
	private Vector2 _scrollPos;

	[MenuItem("ECG/Stat System/Editors/Modifier Editor")]
	public static void Init()
	{

		ModifierDatabaseEditor window = EditorWindow.GetWindow<ModifierDatabaseEditor>();
		window.minSize = new Vector2(800, 400);
		window.Show();
	}
	void OnEnable(){

		if (modifiers == null)
			LoadDatabase();

		state = State.BLANK;
	}

	void OnGUI(){

		EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
		DisplayListArea();
		DisplayMainArea();
		EditorGUILayout.EndHorizontal();
	}

	void LoadDatabase(){

		modifiers = (ModifierDatabase)AssetDatabase.LoadAssetAtPath(DATABASE_PATH, typeof(ModifierDatabase));

		if (modifiers == null)
			CreateDatabase();
	}

	void CreateDatabase(){

		modifiers = ScriptableObject.CreateInstance<ModifierDatabase>();

		AssetDatabase.CreateAsset(modifiers, DATABASE_PATH);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}

	void DisplayListArea()
	{
		EditorGUILayout.BeginVertical(GUILayout.Width(250));
		EditorGUILayout.Space();

		_scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, "box", GUILayout.ExpandHeight(true));


		for (int cnt = 0; cnt < modifiers.COUNT; cnt++)
		{
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("-", GUILayout.Width(25)))
			{
                selectedItem = -1;
                modifiers.RemoveModifierAt(cnt);
				modifiers.SortAlphabeticallyAtoZ();
				EditorUtility.SetDirty(modifiers);
				state = State.BLANK;
				return;
			}

			if (GUILayout.Button(modifiers.GetModifierAt(cnt).name, "box", GUILayout.ExpandWidth(true)))
			{
                newModName = "New Mod";
                newModValue = 0;
                newModType = ModifierType.Flat;
                selectedItem = cnt;
				state = State.EDIT;
			}

			EditorGUILayout.EndHorizontal();
		}

		EditorGUILayout.EndScrollView();

		EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
		EditorGUILayout.LabelField("Items: " + modifiers.COUNT, GUILayout.Width(100));

		if (GUILayout.Button("New Item"))
			state = State.ADD;

		EditorGUILayout.EndHorizontal();
		EditorGUILayout.Space();

		if(GUILayout.Button ("Done"))
		{
            newModName = "New Mod";
            newModValue = 0;
            newModType = ModifierType.Flat;
            EditorUtility.SetDirty (modifiers);
			this.Close ();
		}
		EditorGUILayout.Space ();
		EditorGUILayout.EndVertical();
	}

	void DisplayMainArea(){

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

	void DisplayBlankMainArea(){

		EditorGUILayout.LabelField(
			"There are 3 things that can be displayed here.\n" +
			"1) Item info for editing\n" +
			"2) Blank fields for adding a new Item\n" +
			"3) Blank Area",
			GUILayout.ExpandHeight(true));
	}

	void DisplayEditMainArea()
	{

		modifiers.GetModifierAt (selectedItem).name = EditorGUILayout.TextField(new GUIContent("Name: "), modifiers.GetModifierAt(selectedItem).name);
		modifiers.GetModifierAt (selectedItem).Value = EditorGUILayout.FloatField ("Value: ", modifiers.GetModifierAt (selectedItem).Value);
		modifiers.GetModifierAt (selectedItem).Type = (ModifierType)EditorGUILayout.EnumPopup ("Modifier Type:", modifiers.GetModifierAt (selectedItem).Type);
		modifiers.GetModifierAt (selectedItem).ModifiedStat = (BaseStat)EditorGUILayout.ObjectField ("Modified Stat: ", modifiers.GetModifierAt (selectedItem).ModifiedStat, typeof(BaseStat),false);

		EditorGUILayout.Space();

		if (GUILayout.Button("Done", GUILayout.Width(100)))
		{
            selectedItem = -1;
			modifiers.SortAlphabeticallyAtoZ();
			EditorUtility.SetDirty(modifiers);
			state = State.BLANK;
		}
	}

	void DisplayAddMainArea()
	{

		newModName  = EditorGUILayout.TextField(new GUIContent("Name: "), newModName);
		newModValue = EditorGUILayout.FloatField ("Value: ", newModValue);
		newModType = (ModifierType)EditorGUILayout.EnumPopup ("Modifier Type: ", newModType);
		newModModifiedStat = (BaseStat)EditorGUILayout.ObjectField ("Modified Stat: ", newModModifiedStat, typeof(BaseStat),false);


		if (GUILayout.Button("Done", GUILayout.Width(100)))
		{
			StatModifier newMod = new StatModifier(newModName,newModModifiedStat,newModValue,newModType,null);
			modifiers.AddModifier (newMod);

            newModName = "New Mod";
            newModValue = 0;
            newModType = ModifierType.Flat;
			EditorUtility.SetDirty(modifiers);
			state = State.BLANK;
		}
	}
}

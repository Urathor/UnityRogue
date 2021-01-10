using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;


public class EquipmentDatabaseEditor : EditorWindow
{
	private const string EQ_DATABASE_PATH = @"Assets/Scripts/Resources/Databases/EquipmentDatabase.asset";
	private const string MOD_DATABASE_PATH = @"Assets/Scripts/Resources/Databases/ModifierDatabase.asset";
	private EquipmentDatabase items;
	private Vector2 _scrollPos;

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
    private EquipmentType newItemType;

	private string[] statModType;
	private int statModIndex;
	private List<StatModifier> newItemStatMod;
	private int indexOfMods;
	private int[] newStatIndex;
	private List<StatModifier> editItemStatMods;
	private int[] editStatIndex;

	private ModifierDatabase modifierDatabase;

	[MenuItem("ECG/Item System/Editors/Equipment Editor")]
	public static void Init()
	{

		EquipmentDatabaseEditor window = EditorWindow.GetWindow<EquipmentDatabaseEditor>();
		window.minSize = new Vector2(800, 400);
		window.Show();
	}

	void LoadDatabases(){

		items = (EquipmentDatabase)AssetDatabase.LoadAssetAtPath(EQ_DATABASE_PATH, typeof(EquipmentDatabase));
		modifierDatabase = (ModifierDatabase)AssetDatabase.LoadAssetAtPath (MOD_DATABASE_PATH, typeof(ModifierDatabase));

		if (items == null)
			CreateDatabase();

		if(modifierDatabase == null)
		{
			Debug.LogWarning ("No ModiferDatabase found!!");
		}
	}

	void CreateDatabase(){

		items = ScriptableObject.CreateInstance<EquipmentDatabase>();

		AssetDatabase.CreateAsset(items, EQ_DATABASE_PATH);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}

	void OnEnable()
	{
		if (items == null)
			LoadDatabases();
	
		if(modifierDatabase != null)
		{
			statModType = new string[modifierDatabase.COUNT];
				
			for (int i = 0; i < statModType.Length; i++)
			{
				statModType [i] = modifierDatabase.GetModifierAt (i).name;
			}

			newStatIndex = new int[modifierDatabase.COUNT];
		}
	


		state = State.BLANK;
	}

	void OnGUI(){

		EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
		DisplayListArea();
		DisplayMainArea();
		EditorGUILayout.EndHorizontal();
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
				items.RemoveAt(cnt);
				items.SortAlphabeticallyAtoZ();
				EditorUtility.SetDirty(items);
				state = State.BLANK;
				return;
			}

			if (GUILayout.Button(items.GetEquipmentAt(cnt).IdentifiedName, "box", GUILayout.ExpandWidth(true)))
			{
				selectedItem = cnt;
				editStatIndex = null;
				editItemStatMods = null;
				state = State.EDIT;

			}

			EditorGUILayout.EndHorizontal();
		}

		EditorGUILayout.EndScrollView();

		EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
		EditorGUILayout.LabelField("Items: " + items.COUNT, GUILayout.Width(100));

        if (GUILayout.Button("New Equipment"))
        {
            newItemIcon = null;
            newItemName = "New item name";
            newItemIdentifiedName = "New identified name";
            newItemDescription = "New item descriptino";
            newItemGoldValue = 0;
            newItemType = EquipmentType.Weapon;
            newItemStackable = false;
            newItemStatMod = null;
            state = State.ADD;
        }
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.Space();

		if(GUILayout.Button ("Done"))
		{
			EditorUtility.SetDirty (items);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
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
		items.GetEquipmentAt(selectedItem).ItemIcon = (Sprite)EditorGUILayout.ObjectField ("Item Icon: ", items.GetEquipmentAt(selectedItem).ItemIcon, typeof(Sprite),false);
        //Text Fields
        items.GetEquipmentAt(selectedItem).ItemName = EditorGUILayout.TextField(new GUIContent("Name: "), items.GetEquipmentAt(selectedItem).ItemName);
        items.GetEquipmentAt(selectedItem).IdentifiedName = EditorGUILayout.TextField(new GUIContent("Identifed Name: "), items.GetEquipmentAt(selectedItem).IdentifiedName);
        items.GetEquipmentAt(selectedItem).Description = EditorGUILayout.TextField(new GUIContent("Description: "), items.GetEquipmentAt(selectedItem).Description);

        //ints
        items.GetEquipmentAt(selectedItem).GoldValue = EditorGUILayout.IntField(new GUIContent("Gold Value: "), items.GetEquipmentAt(selectedItem).GoldValue);
        //toggles
        items.GetEquipmentAt(selectedItem).IsIdentified = EditorGUILayout.Toggle("Identifed", items.GetEquipmentAt(selectedItem).IsIdentified);
        items.GetEquipmentAt(selectedItem).Stackable = EditorGUILayout.Toggle ("Stackable", items.GetEquipmentAt(selectedItem).Stackable);

		items.GetEquipmentAt (selectedItem).EquipmentType = (EquipmentType)EditorGUILayout.EnumPopup ("Equipment Type:", items.GetEquipmentAt (selectedItem).EquipmentType);

		if (statModType != null)
		{

			if (items.GetEquipmentAt (selectedItem).StatModifiers == null )
			{

				if (GUILayout.Button ("Add Stat", GUILayout.Width (100)))
				{

					editItemStatMods = new List<StatModifier> ();
					editStatIndex = items.GetEquipmentAt (selectedItem).statIndex = new int[modifierDatabase.COUNT];

					StatModifier newMod = new StatModifier(modifierDatabase.GetModifierAt (0).name,modifierDatabase.GetModifierAt (0).ModifiedStat,
															modifierDatabase.GetModifierAt (0).Value,modifierDatabase.GetModifierAt (0).Type,null);
					editItemStatMods.Add (newMod);
					items.GetEquipmentAt (selectedItem).StatModifiers = editItemStatMods;
				}

			}
			else
			{
				if (editItemStatMods == null)
				{
					editItemStatMods = new List<StatModifier> ();
					editStatIndex = items.GetEquipmentAt (selectedItem).statIndex;

					if(editStatIndex.Length < modifierDatabase.COUNT)
					{
						editStatIndex = new int[modifierDatabase.COUNT];

						for (int i = 0; i < items.GetEquipmentAt (selectedItem).statIndex.Length; i++)
						{	
							editStatIndex [i] = items.GetEquipmentAt (selectedItem).statIndex [i];
						}
					}

					for (int i = 0; i < items.GetEquipmentAt (selectedItem).StatModifiers.Count ; i++)
					{
						editItemStatMods.Add (items.GetEquipmentAt (selectedItem).StatModifiers [i]);
					}
				}
				else
				{
					for (int indexofstats = 0; indexofstats < editItemStatMods.Count; indexofstats++)
					{

						EditorGUILayout.BeginHorizontal ();

						editStatIndex[indexofstats] = EditorGUILayout.Popup (editStatIndex [indexofstats], statModType);

						EditorGUILayout.EndHorizontal ();
					}

					GUILayout.BeginHorizontal ();

					if (editItemStatMods.Count < modifierDatabase.COUNT)
					{
						if (GUILayout.Button ("Add Stat", GUILayout.Width (100)))
						{
							editStatIndex [items.GetEquipmentAt(selectedItem).StatModifiers.Count] = items.GetEquipmentAt(selectedItem).StatModifiers.Count;

							StatModifier newMod = new StatModifier(modifierDatabase.GetModifierAt (0).name,modifierDatabase.GetModifierAt (0).ModifiedStat,
								modifierDatabase.GetModifierAt (0).Value,modifierDatabase.GetModifierAt (0).Type,null);
							editItemStatMods.Add (newMod);

						}
					}

					if (editItemStatMods.Count > 0)
					{
						if (GUILayout.Button ("Remove Stat", GUILayout.Width (100)))
						{
							editItemStatMods.RemoveAt (editItemStatMods.Count-1);
						}
					}
					GUILayout.EndHorizontal ();						
				}
			}

		}


		EditorGUILayout.Space();
		if (GUILayout.Button("Done", GUILayout.Width(100)))
		{
			items.GetEquipmentAt (selectedItem).StatModifiers = editItemStatMods;
			items.GetEquipmentAt (selectedItem).statIndex = editStatIndex;

			for (int i = 0; i < items.GetEquipmentAt (selectedItem).StatModifiers.Count; i++)
			{
				items.GetEquipmentAt (selectedItem).StatModifiers[i] = modifierDatabase.GetCopyOfModifierAt(items.GetEquipmentAt (selectedItem).statIndex [i]);
				items.GetEquipmentAt (selectedItem).StatModifiers[i].Source = items.GetEquipmentAt (selectedItem).ItemName;
			}
            

            items.SortAlphabeticallyAtoZ();

			EditorUtility.SetDirty(items);
			state = State.BLANK;
		}
	}

	void DisplayAddMainArea()
	{
        newItemIcon = (Sprite)EditorGUILayout.ObjectField("Item Icon: ", newItemIcon, typeof(Sprite), false);
        newItemName = EditorGUILayout.TextField(new GUIContent("Name: "), newItemName);
        newItemIdentifiedName = EditorGUILayout.TextField(new GUIContent("Identified Name: "), newItemIdentifiedName);
        newItemDescription = EditorGUILayout.TextField(new GUIContent("Description: "), newItemDescription);
        newItemGoldValue = EditorGUILayout.IntField(new GUIContent("Gold Value: "), newItemGoldValue);
        newItemIdentified = EditorGUILayout.Toggle("IsIdentified", newItemIdentified);
        newItemStackable = EditorGUILayout.Toggle("Stackable", newItemStackable);
        newItemType = (EquipmentType)EditorGUILayout.EnumPopup ("Equipment Type:", newItemType);

		if(statModType != null)
		{
			if (newItemStatMod == null)
			{

				if (GUILayout.Button ("Add Stat", GUILayout.Width (100))) 
				{

					newItemStatMod = new List<StatModifier> ();
					StatModifier newMod = new StatModifier(modifierDatabase.GetModifierAt (0).name,modifierDatabase.GetModifierAt (0).ModifiedStat,
						modifierDatabase.GetModifierAt (0).Value,modifierDatabase.GetModifierAt (0).Type,null);
					newItemStatMod.Add (newMod);
				}

			} 
			else
			{	
				for (int indexofstats = 0; indexofstats < newItemStatMod.Count; indexofstats++)
				{

					EditorGUILayout.BeginHorizontal ();

					newStatIndex[indexofstats] = EditorGUILayout.Popup (newStatIndex [indexofstats], statModType);

					EditorGUILayout.EndHorizontal ();
				}

				GUILayout.BeginHorizontal ();

				if (newItemStatMod.Count < modifierDatabase.COUNT)
				{
					if (GUILayout.Button ("Add Stat", GUILayout.Width (100)))
					{

						newStatIndex [newItemStatMod.Count] = newItemStatMod.Count;
						StatModifier newMod = new StatModifier(modifierDatabase.GetModifierAt (0).name,modifierDatabase.GetModifierAt (0).ModifiedStat,
							modifierDatabase.GetModifierAt (0).Value,modifierDatabase.GetModifierAt (0).Type,null);
						newItemStatMod.Add (newMod);

					}
				}
				if (newItemStatMod.Count > 0)
				{
					if (GUILayout.Button ("Remove Stat", GUILayout.Width (100)))
					{
						newItemStatMod.RemoveAt (newItemStatMod.Count - 1);
					}
				}
				GUILayout.EndHorizontal ();
			}
		}
		EditorGUILayout.Space();


		if (GUILayout.Button("Done", GUILayout.Width(100)))
		{

            EquipmentItem newItem = new EquipmentItem();
            newItem.ItemIcon = newItemIcon;
            newItem.ItemName = newItemName;
            newItem.IdentifiedName = newItemIdentifiedName;
            newItem.Description = newItemDescription;
            newItem.GoldValue = newItemGoldValue;
            newItem.IsIdentified = newItemIdentified;
            newItem.Stackable = newItemStackable;
            newItem.Type = ItemType.Equipment;
            newItem.EquipmentType = newItemType;

            if (newItemStatMod != null)
			{
				newItem.statIndex = newStatIndex;
				newItem.StatModifiers = newItemStatMod;
				for (int i = 0; i < newItem.StatModifiers.Count; i++)
				{
					newItem.StatModifiers[i] = modifierDatabase.GetCopyOfModifierAt (newItem.statIndex [i]);
					newItem.StatModifiers[i].Source = newItem.ItemName;
				}
			}

			items.AddEquipment (newItem);


			newItemIcon = null;
			newItemName = "";
			newItemStackable = false;
			newItemStatMod = null;
			newItemType = EquipmentType.Weapon;
            newItem.Type = ItemType.Equipment;

            EditorUtility.SetDirty(items);
			state = State.BLANK;
		}
	}

	private T[] GetAtPath<T> (string path)
	{
		ArrayList al = new ArrayList ();
		string[] fileEntries = Directory.GetFiles (Application.dataPath + "/" + path);
		foreach (string fileName in fileEntries)
		{
			int index = fileName.LastIndexOf ("/");
			string localPath = "Assets/" + path;
			if (index > 0)
				localPath += fileName.Substring (index);

			object t = AssetDatabase.LoadAssetAtPath (localPath, typeof(T));

			if (t != null)
				al.Add (t);
		}

		T[] result = new T[al.Count];
		for (int i = 0; i < al.Count; i++)
		{
			result [i] = (T)al [i];
		}

		return result;

	}
}

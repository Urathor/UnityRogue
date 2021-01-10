using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPanel : MonoBehaviour
{
    public static EnemyPanel Instance;
    public Transform ContentParent;

    private List<Monster> Monsters;
    private bool listChanged = true;

    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance.gameObject);

        Instance = this;

        Monsters = new List<Monster>();
    }

    public void AddMonsterToList(Monster monster)
    {
        Monsters.Add(monster);
        listChanged = true;
    }

    public void RemoveMonster(Monster monster)
    {
        Monsters.Remove(monster);
        listChanged = true;
    }

    public void  ClearList()
    {
        Monsters.Clear();
    }

    public void LateUpdate()
    {
        if(listChanged)
        {
            foreach(Monster monster in Monsters)
            {
                MonsterStat newStatDisplay = GameObject.Instantiate(Res.Instance.EnemyStatDisplay).GetComponent<MonsterStat>();
                newStatDisplay.gameObject.transform.SetParent(ContentParent);
                newStatDisplay.SetLabel(monster.DisplayName, monster.Color);
                newStatDisplay.SetFill((float)monster.CurrentHealth /(float) monster.MaxHealth);
            }
            listChanged = false;
        }
    }
}

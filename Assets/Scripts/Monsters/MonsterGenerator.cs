using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterGenerator
{
    public List<MobSpawn> MonsterList;
    private List<KeyValuePair<string, float>> pairs;

    public void SetPairs()
    {
        pairs = new List<KeyValuePair<string, float>>();

        foreach (MobSpawn data in MonsterList)
        {
            KeyValuePair<string, float> newPair = new KeyValuePair<string, float>(data.Name, data.Weight);

            pairs.Add(newPair);
        }
    }

    public MonsterData GetMonsterData()
    {
        float roll = Random.Range(1, 101);
        float weightSum = 0;

        for (int i = 0; i < pairs.Count; i++)
        {
            weightSum += pairs[i].Value;

            if(roll < weightSum)
            {
                return GameManager.Instance.MonsterDatabase.GetItem(pairs[i].Key);
            }
        }

        return null;
    }

}

[System.Serializable]
public class MobSpawn
{
    public string Name;
    public float Weight;

    public MobSpawn(string name, float weight)
    {
        Name = name;
        Weight = weight;
    }
}

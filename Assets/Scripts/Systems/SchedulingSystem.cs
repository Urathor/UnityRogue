using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SchedulingSystem
{
    private int time;
    private readonly SortedDictionary<int, List<IScheduleable>> scheduleables;

    public SchedulingSystem()
    {
        time = 0;
        scheduleables = new SortedDictionary<int, List<IScheduleable>>();
    }

    public void Add(IScheduleable scheduleable)
    {
        int key = time + scheduleable.Time;

        if(!scheduleables.ContainsKey(key))
        {
            scheduleables.Add(key, new List<IScheduleable>());
        }

        scheduleables[key].Add(scheduleable);
    }

    public void Remove(IScheduleable scheduleable)
    {
        KeyValuePair<int, List<IScheduleable>> scheduleablesListFound = new KeyValuePair<int, List<IScheduleable>>(-1, null);

        foreach(var scheduleableList in scheduleables)
        {
            if(scheduleableList.Value.Contains(scheduleable))
            {
                scheduleablesListFound = scheduleableList;
                break;
            }
        }

        if(scheduleablesListFound.Value != null)
        {
            scheduleablesListFound.Value.Remove(scheduleable);
            if(scheduleablesListFound.Value.Count < 1)
            {
                scheduleables.Remove(scheduleablesListFound.Key);
            }
        }
    }


    public IScheduleable Get()
    {
        var firstGroup = scheduleables.First();
        var firstScheduleable = firstGroup.Value.First();

        Remove(firstScheduleable);
        time = firstGroup.Key;
        return firstScheduleable;
    }

    public int GetTime()
    {
        return time;
    }

    public void Clear()
    {
        time = 0;
        scheduleables.Clear();
    }
}

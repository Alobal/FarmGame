using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScheduleDataSO", menuName = "AI/ScheduleDataSO")]
public class ScheduleDataSO : ScriptableObject
{
    public List<ScheduleDetail> data;
    public bool loop = false;
    public int count { get { return data.Count; } }
    public ScheduleDetail this[int index]
    {
        get => data[index];
    }
}

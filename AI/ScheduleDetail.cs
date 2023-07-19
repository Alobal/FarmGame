using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class ScheduleDetail : IComparable<ScheduleDetail>
{
    public int hour, minute;//计划时间
    public int seconds { get => TimeManager.ComputeSeconds(hour,minute); }
    public int priority;
    public string target_scene;
    public Vector2Int target_gridpos;
    public AnimationClip clip_at_target;

    public int CompareTo(ScheduleDetail other)
    {
        if (seconds == other.seconds)
        {
            return priority >= other.priority ? 1 : -1;
        }
        else
            return seconds > other.seconds? 1 : -1;
    }
}

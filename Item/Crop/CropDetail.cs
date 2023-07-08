using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class CropDetail
{
    public int seed_id;//seed在item数据库中的id
    [Header("各生长阶段所需天数")]
    public int[] grow_days;//TODO 如何改为常量 但支持编辑器编辑

    [Header("各生长阶段的Prefab")]
    public GameObject[] prefabs;

    [Header("各生长阶段的sprite")]
    public Sprite[] sprites;

    [Space]
    [Header("可收割工具id")]
    public int[] harvest_toolids;

    [Header("收获时所需工具收割次数")]
    public int[] require_harvest_actions;//每一项与收割工具对应

    [Space]
    [Header("果实信息")]
    public int[] product_itemids, product_min_count,product_max_count;
    [SerializeField]
    public float spawn_radius;//果实生成半径

    [Header("可选项")]
    public bool produce_on_player;
    public bool has_animation;
    public bool has_partical_effect;
    private int[] m_total_grow_days;
    public int[] total_grow_days 
    {
        get
        {
            if (m_total_grow_days == null || m_total_grow_days.Length == 0)
                ComputeTotalDays();
            return m_total_grow_days;
        }
    }

    /// <summary>
    /// 根据当前生长天数计算处于的生长阶段
    /// </summary>
    /// <param name="current_day"></param>
    /// <returns></returns>
    public int ComputeGrowStage(int current_day)
    {
        int i = 0;
        for (; i < total_grow_days.Length; i++)
            if (current_day < total_grow_days[i])
                return i;
        return i;
    }
    public int FindHarvestTool(int tool_id)
    {
        for(int i = 0; i < harvest_toolids.Length; i++)
        {
            if (harvest_toolids[i] == tool_id)
                return i;
        }
        return -1;
    }
    private void ComputeTotalDays()
    {
        m_total_grow_days = new int[grow_days.Length];
        int day_sum = 0;
        for (int i = 0; i < grow_days.Length; i++)
        {
            day_sum += grow_days[i];
            m_total_grow_days[i] = day_sum;
        }
    }

}

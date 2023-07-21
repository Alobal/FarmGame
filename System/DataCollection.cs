using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#region Item
/// <summary>
/// 用于描述一个物体的数据，从ItemObject分离是为了序列化
/// </summary>
[Serializable]
public class ItemDetail
{   
    public int id=0;
    public string item_name;
    public ItemType item_type=ItemType.None;
    public Sprite icon;
    public Sprite world_sprite;
    public string description;
    public int use_radius=5;
    public bool can_pick=true;
    public bool can_drop=true;
    public bool can_carry=true;
    public int price=50;
    [Range(0,1)]
    public float sell_count=0.5f;
    public int sell_price
    {
        //向上取整价值
        get { return (int)System.Math.Ceiling(  price* sell_count); }
    }

}


/// <summary>
/// 描述物体的包裹内状态
/// </summary>
[Serializable]
public struct SlotItem
{
    public int item_id;
    public int item_amount;

}

public enum ItemType
{
    None,
    Seed, Commodity, Furniture,
    Tool,BluePrint,
    ReapableScenery,
}

public enum SlotType
{
    Player,Box,Shop
}

#endregion

#region Player
[Serializable]
public class AnimatorType
{
    public PlayerPart player_part;
    public List<PlayerAction> actions;
    public AnimatorOverrideController override_controller;
}

public enum PlayerPart
{
    Body,Hair,Arm,Tool
}

public enum PlayerAction
{
    Default,Carry,Hoe,Break,Water,Chop,Reap
}

#endregion

public enum Season
{
    Spring,Summer,Autumn,Winter
}

[Serializable]
public class NPCPosition
{
    public Transform npc;
    public string start_scene;
    public Vector2 pos;
}


namespace Map
{
    /// <summary>
    /// 单个Tile的属性，一个Tile可以同时有多种属性：Dig,PlaceFurniture...
    /// </summary>
    [Serializable]
    public class TileDetail
    {
        public Vector2Int cell_pos;//以网格为单位的位置
        public List<TileFlag> flags;
        public float day_from_dig=-1;
        public float day_from_water=-1;
        public bool seeded = false;
        public bool is_timing = false;//是否开启响应时间事件
        public bool can_dig { get { return CheckProperty(TileFlag.Dig) && !is_digged; } }
        public bool can_seed { get { return is_digged && !seeded; } }

        public bool can_water { get { return is_digged && !is_watered; } }
        
        public bool is_digged { get { return day_from_dig >= 0; } }
        public bool is_watered { get { return day_from_water >= 0; } }

        public TileDetail(Vector2Int cell_pos, TileFlag property)
        {
            this.cell_pos = cell_pos;
            flags = new() { property };
        }

        public void StartTimeEvent()
        {
            TimeManager.DayEvent += OnDayEvent;
            is_timing = true;
        }
        public void EndTimeEvent()
        {
            TimeManager.DayEvent -= OnDayEvent;
            is_timing= false;
        }

        private void OnDayEvent(int delta_day)
        {
            if (is_digged)
                day_from_dig += delta_day;
            if(is_watered)
                day_from_water += delta_day;
        }

        public void AddProperty(TileFlag property)
        {
            flags.Add(property);
        }

        public bool CheckProperty(TileFlag property)
        {
            return flags.Contains(property);
        }

    }
    public enum TileFlag
    {
        Dig, PlaceFurniture, NPCObstacle
    }

}


#region EventArgs
public class AfterSceneLoadEventArgs: EventArgs
{
    public string origin_scene;
    public string target_scene;
    public Vector3 target_position;
    public AfterSceneLoadEventArgs (string os,string ts, Vector3 tp)
    {
        origin_scene = os;
        target_scene = ts;
        target_position = tp;
    }
}

public class ClickMouseLeftEventArgs
{
    public Vector3 world_pos;
    public GameObject target_go;

    public ClickMouseLeftEventArgs(Vector3 world_pos, GameObject target_go)
    {
        this.world_pos = world_pos;
        this.target_go = target_go;
    }
}
#endregion



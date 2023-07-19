using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Item 
{
    /// <summary>
    /// 包裹数据管理。
    /// </summary>
    public class PackDataManager : Singleton<PackDataManager>
    {
        [Header("物品源数据")]
        public ItemSourceDataSO source_data;// FIX 怎么确保引用给出的变量不变
        [Header("玩家包裹数据")]
        public ItemPackSO player_bag;//FIX 如何控制UI格子数量和data数量同步

        public static event Action<SlotType, List<SlotItem>> UpdatePackData;

        private void Start()
        {
            UpdatePackData?.Invoke(SlotType.Player,player_bag.slot_datas);
            EditorUtility.SetDirty(player_bag);
        }
        public ItemDetail GetItemDetail(int id)
        {
            return source_data.item_details.Find(x => x.id == id);
        }

        public void AddItemToPlayer(int item_id)
        {
            player_bag.PushItem(item_id);
            //更新UI
            UpdatePackData?.Invoke(SlotType.Player, player_bag.slot_datas);
        }

        public bool RemoveItem(int slot_index,int amount=1)
        {
            if (player_bag.RemoveItem(slot_index, amount))
            {
                UpdatePackData.Invoke(SlotType.Player, player_bag.slot_datas);
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// 检查Slot内的物品容量是否为0
        /// </summary>
        /// <param name="slot_i"></param>
        /// <returns></returns>
        public bool IsSlotEmpty(int slot_i)
        {
            return player_bag.slot_datas[slot_i].item_amount <= 0;
        }

        public void SwapSlotData(SlotUI source, SlotUI target)
        {

            if(source.slot_type == SlotType.Player && target.slot_type == SlotType.Player)
            {
                var temp = player_bag.slot_datas[source.index];
                player_bag.slot_datas[source.index] = player_bag.slot_datas[target.index];
                player_bag.slot_datas[target.index] = temp;
            }

            UpdatePackData?.Invoke(SlotType.Player, player_bag.slot_datas);

        }
    }
}


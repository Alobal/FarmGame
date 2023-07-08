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
        [Header("包裹数据")]
        public ItemPackSO bag_data;//FIX 如何控制UI格子数量和data数量同步

        public static event Action<SlotType> UpdatePackData;

        private void Start()
        {
            UpdatePackData?.Invoke(SlotType.Player);
            EditorUtility.SetDirty(bag_data);
        }
        public ItemDetail GetItemDetail(int id)
        {
            return source_data.item_details.Find(x => x.id == id);
        }

        public void AddItemToPlayer(int item_id)
        {
            bag_data.PushItem(item_id);
            //更新UI
            UpdatePackData?.Invoke(SlotType.Player);
        }

        public bool RemoveItem(int slot_index,int amount=1)
        {
            if (bag_data.RemoveItem(slot_index, amount))
            {
                UpdatePackData.Invoke(SlotType.Player);
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
            return bag_data.slot_datas[slot_i].item_amount <= 0;
        }

        public void SwapSlotData(SlotUI source, SlotUI target)
        {

            if(source.slot_type == SlotType.Player && target.slot_type == SlotType.Player)
            {
                var temp = bag_data.slot_datas[source.index];
                bag_data.slot_datas[source.index] = bag_data.slot_datas[target.index];
                bag_data.slot_datas[target.index] = temp;
            }

            UpdatePackData?.Invoke(SlotType.Player);

        }
    }
}


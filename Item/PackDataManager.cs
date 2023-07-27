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
        public ItemSourceDataSO item_data;// FIX 怎么确保引用给出的变量不变
        public BluePrintDataSO blueprint_data;
        [Header("玩家包裹数据")]
        public ItemPackSO player_bag;//FIX 如何控制UI格子数量和data数量同步
        [HideInInspector]public ItemPackSO other_bag;//客体背包数据，每次打开商店或仓库时填充引用，关闭时清除引用

        public static event Action<SlotType, ItemPackSO> UpdatePackData;//数据变动事件

        private void Start()
        {
            UpdatePackData?.Invoke(SlotType.Player,player_bag);
            EditorUtility.SetDirty(player_bag);
        }
        public ItemDetail GetItemDetail(int id)
        {
            return item_data.item_details.Find(x => x.id == id);
        }

        public BluePrintDetail GetBluePrintDetail(int id)
        {
            return blueprint_data.GetBluePrint(id);
        }

        #region PlayerBag 操作

        public void PlayerAddItem(int item_id)
        {
            player_bag.PushItem(item_id);
            //更新UI
            UpdatePackData?.Invoke(SlotType.Player, player_bag);
        }

        public bool PlayerRemoveItemAt(int slot_index,int amount=1)
        {
            if (player_bag.RemoveItemAt(slot_index, amount))
            {
                UpdatePackData?.Invoke(SlotType.Player, player_bag);
                return true;
            }
            else
                return false;
        }

        public bool PlayerRemoveItem(int item_id,int amount)
        {
            int slot_index = player_bag.FindItem(item_id);
            if(slot_index!=-1)//存在该物品
            {

                return PlayerRemoveItemAt(slot_index, amount);
            }
            else
                return false;
        }

        #endregion

        /// <summary>
        /// 检查Slot内的物品容量是否为0
        /// </summary>
        /// <param name="slot_i"></param>
        /// <returns></returns>
        public bool IsSlotEmpty(SlotType slot_type,int slot_i)
        {
            var bag = GetPack(slot_type);
            return bag.slot_datas[slot_i].item_amount <= 0;
        }

        //交换两个slot的数据，并刷新UI显示。仅对非交易的SlotType有效。
        public void SwapSlotData(SlotUI source, SlotUI target)
        {
            var source_bag = GetPack(source.slot_type);
            var target_bag = GetPack(target.slot_type);

            var temp = source_bag.slot_datas[source.index];
            source_bag.slot_datas[source.index] = target_bag.slot_datas[target.index];
            target_bag.slot_datas[target.index] = temp;
            UpdatePackData?.Invoke(SlotType.Player, player_bag);
            if (source.slot_type == SlotType.Box || target.slot_type == SlotType.Box)
                UpdatePackData?.Invoke(SlotType.Box, other_bag);

        }

        public bool TradeSlotData(SlotUI source, SlotUI target,int amount)
        {
            ItemPackSO source_bag=GetPack(source.slot_type);
            ItemPackSO target_bag =GetPack(target.slot_type);
            int cost = 0;
            if(source.slot_type == SlotType.Shop)//买
            {
                if(amount> source_bag[source.index].item_amount)
                {
                    Debug.Log("商店数量不足");
                    return false;
                }
                cost = source.item_detail.price * amount;
                if(cost > player_bag.money)
                {
                    Debug.Log("玩家金钱不足");
                    return false;
                }
            }
            else if(target.slot_type == SlotType.Shop)//卖
            {
                cost= -source.item_detail.sell_price * amount;//负数代表增加金额
            }
            //可成功交易
            target_bag.PushItem(source.item_detail.id, amount);
            source_bag.RemoveItemAt(source.index,amount);
            player_bag.money -= cost;

            UpdatePackData?.Invoke(SlotType.Player, player_bag);
            UpdatePackData?.Invoke(SlotType.Shop, other_bag);
            return true;
        }

        private SlotItem SlotToData(SlotUI slot)
        {
            return GetPack(slot.slot_type)[slot.index];
        }

        private ItemPackSO GetPack(SlotType slot_type)
        {
            return slot_type == SlotType.Player ? player_bag : other_bag;
        }
    }
}


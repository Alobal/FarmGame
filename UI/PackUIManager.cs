using Item;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Item
{
    public class PackUIManager : Singleton<PackUIManager>
    {
        [SerializeField] private SlotUI[] player_slots;//action_bar + player_bag
        [SerializeField] private GameObject player_bag;
        //客体背包管理
        [SerializeField] private GameObject slot_prefab;//用于动态生成slot
        [SerializeField] private GameObject other_bag;
        private SlotUI[] other_slots;//other_bag
        public ItemToolTipUI item_tooltip;
        [HideInInspector]
        public Image drag_image;//由对应slot填充拖拽显示的图片

        private void Start()
        {
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
                SwitchPlayerBag();
        }

        private void OnEnable()//NOTE onEnable 不是在所有物体都awake之后！每个物体自己awake然后onEnable
        {
            PackDataManager.UpdatePackData += OnUpdatePackData;
            SlotUI.ClickSlot += OnClickSlot;
        }

        private void OnDisable()
        {
            PackDataManager.UpdatePackData -= OnUpdatePackData;
            SlotUI.ClickSlot -= OnClickSlot;
        }



        /// <summary>
        /// 更新packdata时，UI系统重新读取所有SlotUI。
        /// </summary>
        /// <param name="slot_type"></param>
        private void OnUpdatePackData(SlotType slot_type,List<SlotItem> datas)
        {
            var slots= GetSlots(slot_type);
            int length = slots.Length;
            Debug.Assert(length == datas.Count, $"UI Slot length {length} and Data Slot length {datas.Count} is different.");
            for (int i=0;i<length;i++)
            {
                slots[i].Init(datas[i].item_id, datas[i].item_amount,i);
            }
            ClearOldSelect(-1,slot_type);
        }

        public void SwitchPlayerBag()
        {
            if (player_bag.activeInHierarchy)
                player_bag.SetActive(false);
            else
                player_bag.SetActive(true);
        }

        /// <summary>
        /// 清除非select_i位置的Slot的select状态
        /// </summary>
        /// <param name="select_i"></param>
        public void ClearOldSelect(int select_i, SlotType slot_type)
        {
            var slots= GetSlots(slot_type);
            for (int i=0;i< slots.Length;i++)
            {
                if (i != select_i)
                    slots[i].ClearSelect();
            }
        }
        private void OnClickSlot(SlotUI select_slot)
        {
            ClearOldSelect(select_slot.index,select_slot.slot_type);
        }

        #region 管理客体背包
        public void OpenOtherBag(List<SlotItem> data)
        {
            other_bag.gameObject.SetActive(true);
            other_slots=new SlotUI[data.Count];
            for(int i=0;i<other_slots.Length;i++)
            {
                var slot= Instantiate(slot_prefab, other_bag.transform.GetChild(1)).GetComponent<SlotUI>();
                slot.slot_type = SlotType.Shop;
                other_slots[i]=slot;
            }

            OnUpdatePackData(SlotType.Shop, data);
        }
        #endregion

        private SlotUI[] GetSlots(SlotType slot_type)
        {
            return slot_type == SlotType.Player ? player_slots : other_slots;
        }
    }
}

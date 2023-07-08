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
        [SerializeField] private SlotUI[] slots;//action_bar + bag
        [SerializeField] private GameObject bag_ui;
        public ItemToolTipUI item_tooltip;
        [Header("拖拽变量")]
        public Image drag_image;

        private void Start()
        {
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
                SwitchBagShow();
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
        private void OnUpdatePackData(SlotType slot_type)
        {
            List<SlotItem> datas = PackDataManager.instance.bag_data.slot_datas;
            int length = slots.Length;
            Debug.Assert(length == datas.Count, $"UI Slot length {length} and Data Slot length {datas.Count} is different.");
            for (int i=0;i<length;i++)
            {
                slots[i].Init(datas[i].item_id, datas[i].item_amount,i);
            }
            ClearOldSelect(-1);
        }

        public void SwitchBagShow()
        {
            if (bag_ui.activeInHierarchy)
                bag_ui.SetActive(false);
            else
                bag_ui.SetActive(true);
        }

        /// <summary>
        /// 清除非select_i位置的Slot的select状态
        /// </summary>
        /// <param name="select_i"></param>
        public void ClearOldSelect(int select_i)
        {
            
            for(int i=0;i< slots.Length;i++)
            {
                if (i != select_i)
                    slots[i].ClearSelect();
            }
        }
        private void OnClickSlot(SlotUI select_slot)
        {
            ClearOldSelect(select_slot.index);
        }
    }



}

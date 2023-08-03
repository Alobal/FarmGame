using Item;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Item
{
    public class PackUIManager : Singleton<PackUIManager>
    {
        //玩家背包管理
        [SerializeField] private SlotUI[] player_slots;//action_bar + player_bag
        [SerializeField] private GameObject player_bag;
        [SerializeField] private TextMeshProUGUI money;
        //客体背包管理
        [SerializeField] private GameObject slot_prefab;//用于动态生成slot
        [SerializeField] private GameObject other_bag;
        private SlotUI[] other_slots;//other_bag
        //通用管理
        public ItemToolTipUI item_tooltip;
        [HideInInspector]
        public Image drag_image;//由对应slot填充拖拽显示的图片
        public TradeUI trade_ui;

       
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
        /// 更新packdata时，UI系统刷新所有SlotUI的显示。
        /// </summary>
        /// <param name="slot_type"></param>
        private void OnUpdatePackData(SlotType slot_type,ItemPackSO pack)
        {
            List<SlotItem> datas = pack.slot_datas;
            var slots= GetSlots(slot_type);
            int ui_length = slots.Length;

            //检查UI格子和数据格子的数量是否统一
            if(slot_type == SlotType.Player)//玩家背包必须统一
                Debug.Assert(ui_length == datas.Count, $"UI Slot length {ui_length} and Data Slot length {datas.Count} is different.");
            else if(ui_length != datas.Count && pack.can_expend)//其他背包可以允许扩展格子
            {
                CloseOtherBag();
                OpenOtherBag(pack);
            }

            //更新所有UI格子的数据
            for (int i=0;i<ui_length;i++)
            {
                slots[i].Init(datas[i].item_id, datas[i].item_amount,i);
            }
            money.text= PackDataManager.instance.player_bag.money.ToString();
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
        public void OpenOtherBag(ItemPackSO pack)
        {
            List<SlotItem> data = pack.slot_datas;
            other_bag.gameObject.SetActive(true);
            other_slots=new SlotUI[data.Count];
            for(int i=0;i<other_slots.Length;i++)
            {
                var slot= Instantiate(slot_prefab, other_bag.transform.GetChild(1)).GetComponent<SlotUI>();
                slot.slot_type = SlotType.Shop;
                other_slots[i]=slot;
            }
            //刷新布局，布局不会自动立即重绘。
            LayoutRebuilder.ForceRebuildLayoutImmediate(other_bag.GetComponent<RectTransform>());
            OnUpdatePackData(SlotType.Shop, pack);
        }

        public void CloseOtherBag()
        {
            other_bag.gameObject.SetActive(false);
            for (int i = 0; i < other_slots.Length; i++)
            {
                Destroy(other_slots[i].gameObject);
            }
        }
        #endregion

        private SlotUI[] GetSlots(SlotType slot_type)
        {
            return slot_type == SlotType.Player ? player_slots : other_slots;
        }
    }
}

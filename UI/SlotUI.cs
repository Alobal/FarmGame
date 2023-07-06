using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Item
{
    public class SlotUI : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler,
                        IPointerEnterHandler, IPointerExitHandler
    {
        [Header("显示组件")]
        [SerializeField] private Image item_image;
        [SerializeField] private TextMeshProUGUI amount_text;
        [SerializeField] private GameObject highlight;
        [SerializeField] private Button button;

        [Header("格子状态")]
        public int index;//slot index in the manager
        public ItemDetail item_detail;//注意被序列化后初始状态不为null
        public static event Action<SlotUI> ClickSlot;//物品栏选择事件

        //序列化属性需要添加 field:
        [field: SerializeField] public SlotType slot_type { get; set; }
        public bool is_empty { get { return PackDataManager.instance.IsSlotEmpty(index); } }
        public bool is_selected
        {
            get { return highlight.activeInHierarchy; }
            private set
            {
                highlight.SetActive(value);
            }
        }

        private void Start()
        {
            is_selected = false;
        }

        public void Init(int item_id, int amount, int index)
        {
            this.index = index;
            var new_item_detail = PackDataManager.instance.GetItemDetail(item_id);
            if (new_item_detail == null || amount == 0)
                MakeEmpty();
            else
                MakeContent(new_item_detail, amount);
        }
        /// <summary>
        /// 给UI生成Item
        /// </summary>
        /// <param name="new_detail">新的Item的信息</param>
        /// <param name="amount">生成数量</param>
        public void MakeContent(ItemDetail new_detail, int amount)
        {
            item_detail = new_detail;
            item_image.enabled = true;
            item_image.sprite = new_detail.icon;
            amount_text.text = amount.ToString();
            button.interactable = true;
        }
        public void MakeEmpty()
        {
            item_detail = null;
            item_image.enabled = false;
            item_image.sprite = null;
            amount_text.text = string.Empty;
            button.interactable = false;
            is_selected = false;
        }

        public void ClearSelect()
        {
            is_selected = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            is_selected = !is_selected;
            ClickSlot.Invoke(this);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (is_empty) return;
            PackUIManager.instance.drag_image.enabled = true;
            PackUIManager.instance.drag_image.sprite = item_image.sprite;
            PackUIManager.instance.drag_image.transform.position = transform.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (is_empty) return;
            PackUIManager.instance.drag_image.transform.position += (Vector3)eventData.delta;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (is_empty) return;
            PackUIManager.instance.drag_image.enabled = false;
            PackUIManager.instance.drag_image.sprite = null;
            //击中UI
            if (eventData.pointerCurrentRaycast.gameObject is GameObject go)
            {
                if (go.GetComponent<SlotUI>() is SlotUI target_slot)
                {
                    PackDataManager.instance.SwapSlotData(this, target_slot);
                }
            }
            else//没有击中UI，即鼠标落在地图上
            {
                if (item_detail.can_drop)
                {
                    int keep_id = item_detail.id;//提前备份id，以防最后一个物品扔掉后格子清空
                    PackDataManager.instance.RemoveItem(index, 1);
                    Vector3 world_pos = Camera.main.ScreenToWorldPoint(eventData.position);
                    world_pos.z = 0;
                    WorldItemManager.instance.MakeItem(keep_id, world_pos);
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (is_empty) return;
            PackUIManager.instance.item_tooltip.Init(item_detail.id, slot_type, transform.position);
            PackUIManager.instance.item_tooltip.gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (is_empty) return;
            PackUIManager.instance.item_tooltip.gameObject.SetActive(false);
        }
    }
}

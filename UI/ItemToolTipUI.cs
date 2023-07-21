using Item;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemToolTipUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI item_name;
    [SerializeField] private TextMeshProUGUI item_type;
    [SerializeField] private TextMeshProUGUI item_desc;
    [SerializeField] private TextMeshProUGUI item_price;
    [SerializeField] private ResourcePanelUI resource_panel;

    /// <summary>
    /// 获取Slot物品信息进行Tip显示
    /// </summary>
    /// <param name="item_id"></param>
    /// <param name="slot_type"></param>
    /// <param name="pos"></param>
    public void Init(int item_id,SlotType slot_type,Vector3 pos)
    {
        var item_detail = PackDataManager.instance.GetItemDetail(item_id);
        if (item_detail == null) return;

        item_name.text = item_detail.item_name;
        item_type.text = item_detail.item_type.ToText();
        item_desc.text = item_detail.description;

        int price = slot_type == SlotType.Player ? item_detail.sell_price : item_detail.price;
        item_price.text = price != 0 ?price.ToString() : "----";

        if(item_detail.item_type==ItemType.BluePrint)//蓝图物品需要额外显示资源信息
        {
            if(PackDataManager.instance.GetBluePrintDetail(item_id) is BluePrintDetail blueprint_detail )
                resource_panel.Open(blueprint_detail);
        }
        else
        {
            resource_panel.Close();
        }

        transform.position = AdjustPosInScreen(pos);

    }



    /// <summary>
    /// 自适应调整Tip窗口位置，避免Tip窗口超出屏幕
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private Vector3 AdjustPosInScreen(Vector3 pos)
    {
        var rect=GetComponent<RectTransform>();
        float exceed_height = pos.y - rect.sizeDelta.y * rect.localScale.y;//向下超出 负数
        float exceed_weight = pos.x + rect.sizeDelta.x * rect.localScale.x - Screen.width;
        exceed_weight = Math.Max(exceed_weight, 0);
        exceed_height = Math.Min(exceed_height, 0);
        pos = new Vector3(pos.x - exceed_weight, pos.y - exceed_height, pos.z);
        return pos;
    }
}

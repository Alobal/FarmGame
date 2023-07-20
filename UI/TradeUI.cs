using Item;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradeUI : MonoBehaviour
{
    public Image item_image;
    public TextMeshProUGUI item_name;
    public TMP_InputField input_amount;
    public SlotUI source;
    public SlotUI target;

    public void Init(SlotUI source_slot,SlotUI target_slot)
    {
        gameObject.SetActive(true);
        this.source = source_slot;
        this.target = target_slot;
        item_image.sprite = source.item_detail.icon;
        item_name.text = source.item_detail.item_name;
        input_amount.text = string.Empty;
    }

    public void Submit()
    {
        PackDataManager.instance.TradeSlotData(source, target, int.Parse(input_amount.text));
        Cancel();
    }

    public void Cancel()
    {
        gameObject.SetActive(false);
        source = null;
        target = null;
        item_image.sprite = null;
        item_name.text = string.Empty;
        input_amount.text = string.Empty;
    }
}

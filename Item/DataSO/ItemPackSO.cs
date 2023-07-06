using Item;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="ItemPack",menuName ="Item/ItemPackSO")]
public class ItemPackSO : ScriptableObject
{
    public List<SlotItem> slot_datas;

    /// <summary>
    /// 在对应index的Slot中移除Item。如果移除前数量<=0则返回fasle。
    /// </summary>
    /// <param name="index"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    public bool RemoveItem(int index,int amount)
    {
        if (slot_datas[index].item_amount <= 0)
            return false;

        var new_item = slot_datas[index];
        new_item.item_amount -= amount;
        if (new_item.item_amount <= 0)
            new_item.item_id = 0;
        slot_datas[index]= new_item;
        return true;
    }

    /// <summary>
    /// 在背包内存储item。优先叠放同类物体，否则寻找空位存放。
    /// </summary>
    /// <param name="add_item">需要添加的物体，不会引用</param>
    /// <param name="amount"></param>
    /// <returns></returns>
    public bool PushItem(int item_id,int amount=1)
    {
        int push_index = FindItem(item_id);
        if (push_index == -1) push_index = FindEmpty();
        return PushItemAtIndex(item_id, push_index,amount);
    }

    public bool PushItemAtIndex(int item_id,int index,int amount)
    {
        if (index == -1) return false;

        //该位置已有物体且不是同一类
        if (slot_datas[index].item_amount != 0 && slot_datas[index].item_id != item_id)
            return false;

        //允许插入 注意list的索引器返回值类型，局部修改list[i].x无意义
        SlotItem new_item = new()
        {
            item_id = item_id,
            item_amount = amount + slot_datas[index].item_amount
        };
        slot_datas[index]= new_item;
        return true;
    }

    /// <summary>
    /// 找到一个空位置，返回index
    /// </summary>
    /// <returns></returns>
    public int FindEmpty()
    {
        for (int i = 0; i <slot_datas.Count; i++)
        {
            if (slot_datas[i].item_amount == 0)
                return i;
        }
        return -1;
    }
    /// <summary>
    /// 找到id物体的位置，返回index
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private int FindItem( int id)
    {
        for (int i = 0; i < slot_datas.Count; i++)
        {
            if (slot_datas[i].item_id == id)
                return i;
        }
        return -1;
    }

    public SlotItem this[int i]
    {
        get { return slot_datas[i]; }
        set { slot_datas[i] = value; }
    }
}

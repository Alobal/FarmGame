using Item;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//用于建造家具的蓝图
[CreateAssetMenu(fileName = "BluePrintDataSO",menuName = "Item/BluePrintDataSO")]
public class BluePrintDataSO : ScriptableObject
{
    public List<BluePrintDetail> data;

    public BluePrintDetail GetBluePrint(int blueprint_id)
    {
        return data.Find(x => x.blueprint_id == blueprint_id);
    }

}

[System.Serializable]
public class BluePrintDetail
{
    public int blueprint_id;
    public SlotItem[] resources;
    public int product_id;//仅限生成家具
}

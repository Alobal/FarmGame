using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//FIX: 如何保护内容不被修改
[CreateAssetMenu(fileName ="ItemSourceDataSO",menuName = "Item/ItemSourceDataSO")]
public class ItemSourceDataSO: ScriptableObject
{
    public List<ItemDetail> item_details;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//FIX: 如何保护内容不被修改
[CreateAssetMenu(fileName = "CropSourceDataSO", menuName = "Item/CropSourceDataSO")]
public class CropSourceDataSO: ScriptableObject
{
    public List<CropDetail> crop_details;
}
   


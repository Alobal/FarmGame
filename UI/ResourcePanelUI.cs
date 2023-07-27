using Item;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//蓝图所需资源的显示UI
public class ResourcePanelUI : MonoBehaviour
{
    [SerializeField] private Image[] resource_images = new Image[4];
    [SerializeField] private TextMeshProUGUI[] resource_amounts = new TextMeshProUGUI[4];


    /// <summary>
    /// 给定position打开面板。
    /// </summary>
    /// <param name="blueprint_detail"></param>
    /// <param name="position">注意如果Canvas是屏幕空间，则position需要是屏幕坐标。</param>
    public void Open(BluePrintDetail blueprint_detail,Vector3 position)
    {
        transform.position= position;
        Open(blueprint_detail);
    }

    public void Open(BluePrintDetail blueprint_detail)
    {
        gameObject.SetActive(true);
        var resources = blueprint_detail.resources;
        for (int i = 0; i < resource_images.Length; i++)
        {
            if (i < resources.Length && resources[i].item_amount>0)
            {
                var icon = PackDataManager.instance.GetItemDetail(resources[i].item_id).icon;
                resource_images[i].gameObject.SetActive(true);
                resource_images[i].sprite = icon;
                resource_amounts[i].text = resources[i].item_amount.ToString();
            }
            else
            {
                resource_images[i].gameObject.SetActive(false);
            }
        }
    }

    public void Close()
    {
        if(gameObject.activeSelf)
            gameObject.SetActive(false);
    }
}

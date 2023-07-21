using Item;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ResourcePanelUI : MonoBehaviour
{
    [SerializeField] private Image[] resource_images = new Image[4];
    [SerializeField] private TextMeshProUGUI[] resource_amounts = new TextMeshProUGUI[4];


    public void Open(BluePrintDetail blueprint_detail,Vector3 position)
    {
        transform.position= position;
        Open(blueprint_detail);
    }
    //为蓝图item填充所需资源的面板
    public void Open(BluePrintDetail blueprint_detail)
    {
        gameObject.SetActive(true);
        var resources = blueprint_detail.resources;
        for (int i = 0; i < resource_images.Length; i++)
        {
            if (i < resources.Length)
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
        gameObject.SetActive(false);
    }
}

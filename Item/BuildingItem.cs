using Item;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO 和Itemobject整合起来
[RequireComponent(typeof(SpriteRenderer),typeof(BoxCollider2D))]
public class BuildingItem : MonoBehaviour
{
    public int blueprint_id;
    private BluePrintDetail blueprint;
    [SerializeField] private SpriteRenderer sprite_renderer;
    [SerializeField] private BoxCollider2D collide;
    private SlotItem[] need_resource;
    bool player_near = false;

    private bool is_complete { get => Array.FindIndex(need_resource, (x => x.item_amount > 0)) == -1; }

    private void Start()
    {
        if (blueprint_id != 0)
            Init();
    }

    private void Update()
    {
        if(player_near && Input.GetKeyDown(KeyCode.Space))
        {
            GetResource();
        }
    }

    private void GetResource()
    {
        int index = Array.FindIndex(need_resource, (x => x.item_amount > 0));//need_resource的索引
        if (index < 0)
            return;
        if (PackDataManager.instance.PlayerRemoveItem(need_resource[index].item_id, 1))
        {
            need_resource[index].item_amount-= 1;
            OpenResourcePanel();
            if (is_complete)
                Complete();
        }
        else
        {
            Debug.Log("资源不足");
        }
    }



    private void Complete()
    {
        WorldItemManager.instance.MakeItem(blueprint.product_id, transform.position);
        Destroy(gameObject);
    }

    public void Init()
    {
        blueprint=PackDataManager.instance.GetBluePrintDetail(blueprint_id);
        ItemDetail product_detail=PackDataManager.instance.GetItemDetail(blueprint.product_id);
        sprite_renderer.sprite = product_detail.world_sprite;
        need_resource = new SlotItem[blueprint.resources.Length];
        blueprint.resources.CopyTo(need_resource,0);
        UtilityMethods.AdaptiveBoxColliderToSprite(sprite_renderer.sprite, collide);
    }

    private void OpenResourcePanel()
    {
        GeneralUIManager.instance.resource_panel.Close();

        BluePrintDetail bp_detail = new BluePrintDetail();
        bp_detail.resources = need_resource;
        Vector3 panel_pos = transform.position + Vector3.up * sprite_renderer.sprite.bounds.size.y * 1.1f;
        GeneralUIManager.instance.resource_panel.Open(bp_detail, panel_pos);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        OpenResourcePanel();
        player_near = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GeneralUIManager.instance.resource_panel.Close();
        player_near = false;

    }

}

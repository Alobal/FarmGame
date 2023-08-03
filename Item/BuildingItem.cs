using Item;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO 和Itemobject整合起来

public class BuildingItem : ItemObject
{
    private BluePrintDetail blueprint;
    public SlotItem[] need_resource;
    bool player_near = false;

    private bool is_complete { get => Array.FindIndex(need_resource, (x => x.item_amount > 0)) == -1; }

    protected override void Start()
    {
        if (init_id != 0)
            InitInternal();
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
        WorldItemManager.instance.Remove(this);
    }

    protected override void InitInternal()
    {
        blueprint=PackDataManager.instance.GetBluePrintDetail(init_id);
        ItemDetail product_detail=PackDataManager.instance.GetItemDetail(blueprint.product_id);

        sprite_render.sprite = product_detail.world_sprite;
        if (need_resource == null || need_resource.Length == 0)
        {
            need_resource = new SlotItem[blueprint.resources.Length];
            blueprint.resources.CopyTo(need_resource, 0);
        }
        UtilityMethods.AdaptiveBoxColliderToSprite(sprite_render.sprite, collide);
    }

    private void OpenResourcePanel()
    {
        GeneralUIManager.instance.resource_panel.Close();

        BluePrintDetail bp_detail = new BluePrintDetail();
        bp_detail.resources = need_resource;
        Vector3 panel_pos = transform.position + Vector3.up * sprite_render.sprite.bounds.size.y * 1.1f;
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

    public new SaveData GetSaveData()
    {
        return new SaveData()
        {
            blueprint_id = this.init_id,
            need_resource = this.need_resource,
            pos = transform.position
        };
    }

    public new struct SaveData
    {
        public int blueprint_id;
        public SerialVec3 pos;
        public SlotItem[] need_resource;
    }

}

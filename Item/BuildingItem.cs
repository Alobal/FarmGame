using Item;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer),typeof(BoxCollider2D))]
public class BuildingItem : MonoBehaviour
{
    public int blueprint_id;
    private BluePrintDetail blueprint;
    [SerializeField] private SpriteRenderer sprite_renderer;
    [SerializeField] private BoxCollider2D collide;
    private SlotItem[] need_resource;

    private void Start()
    {
        if (blueprint_id != 0)
            Init();
    }

    public void Init()
    {
        blueprint=PackDataManager.instance.GetBluePrintDetail(blueprint_id);
        ItemDetail product_detail=PackDataManager.instance.GetItemDetail(blueprint.product_id);
        sprite_renderer.sprite = product_detail.world_sprite;
        sprite_renderer.color = new(1, 1, 1, 0.5f);
        need_resource = new SlotItem[blueprint.resources.Length];
        blueprint.resources.CopyTo(need_resource,0);
        UtilityMethods.AdaptiveBoxColliderToSprite(sprite_renderer.sprite, collide);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        BluePrintDetail bp_detail = new BluePrintDetail();
        bp_detail.resources= need_resource;
        Vector3 panel_pos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 4);
        GeneralUIManager.instance.resource_panel.Open(bp_detail, panel_pos);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GeneralUIManager.instance.resource_panel.Close();
    }

}

using Item;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 根据动作状态更改Player的动画
/// </summary>
public class AnimatorOverride : MonoBehaviour
{
    public SpriteRenderer hold_sprite;
    public Dictionary<string,Animator> origin_animators=new ();
    public List<AnimatorType> override_list;
    private void Awake()
    {
        var animators = GetComponentsInChildren<Animator>();
        foreach (var animator in animators)
        {
            origin_animators.Add(animator.name,animator);
        }
    }

    private void OnEnable()
    {
        SlotUI.ClickSlot += OnSelectSlot;
    }

    private void OnDisable()
    {
        SlotUI.ClickSlot -= OnSelectSlot;
    }

    private void OnSelectSlot(SlotUI slot)
    {
        if (slot.slot_type != SlotType.Player) 
            return;

        
        PlayerAction action;//获取物品对应action
        if (!slot.is_selected || slot.is_empty)//没有选中 或 选中空格子，则默认动作
            action = PlayerAction.Default;
        else//WORKFLOW
            action = slot.item_detail.item_type switch
            {
                ItemType.Seed => PlayerAction.Carry,
                ItemType.Commodity => PlayerAction.Carry,
                ItemType.Tool when slot.item_detail.id==1011 => PlayerAction.Hoe,
                ItemType.Tool when slot.item_detail.id == 1012 => PlayerAction.Water,
                ItemType.Tool when slot.item_detail.id == 1001 => PlayerAction.Chop,
                ItemType.Tool when slot.item_detail.id == 1002 => PlayerAction.Reap,
                _ => PlayerAction.Default
            };
        //Carry动作额外处理 显示物品
        if (action == PlayerAction.Carry)
        {
            hold_sprite.sprite = slot.item_detail.world_sprite;
            hold_sprite.enabled = true;
        }
        SwitchAnimator(action);
    }

    //按照player action切换所有对应PlayerPart的override animator
    public void SwitchAnimator(PlayerAction action)
    {
        //非carry动作时关闭carry sprite
        if(action!=PlayerAction.Carry)
        {
            hold_sprite.sprite = null;
            hold_sprite.enabled = false;
        }
        foreach (var new_controller in override_list)
        {
            if(new_controller.actions.Contains(action))//如果匹配
            {   //覆写对应part的动画控制器
                origin_animators[new_controller.player_part.ToString()].runtimeAnimatorController = new_controller.override_controller;
            }
        }
    }
}

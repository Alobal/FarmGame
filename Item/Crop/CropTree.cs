using Item;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Crop
{
    public class CropTree : CropObject
    {
        protected Animator animator;
        //NOTE 子类有则覆盖基类，不会主动调用基类，除非base。子类没有则会自动调用基类，即使基类是private
        protected override void Start()
        {
            animator=GetComponentInChildren<Animator>();
            base.Start();
        }
        /// <summary>
        /// 根据current_day 更新当前植物的生长状态
        /// </summary>
        public override void Harvest(int tool_id)
        {
            Debug.Log("is haravesting...");
            //单次收割
            CheckHarvestable(tool_id);
            int tool_index = crop_detail.FindHarvestTool(tool_id);
            harvest_action_count[tool_index] += 1;

            if (animator!=null)
            {
                bool left_or_right=UtilityMethods.RandomBool();
                if (left_or_right)
                    animator.SetTrigger("RotateRight");
                else
                    animator.SetTrigger("RotateLeft");
            }
            if(particle != null)
            {
                ParticleSystem particle_object=ObjectPoolManager.instance.Get(particle, particle_pos,1.0f)
                                                .GetComponent<ParticleSystem>();
            }

            //收割次数满足 实现收获
            if (harvest_action_count[tool_index] >= crop_detail.require_harvest_actions[tool_index])
            {
                if (animator != null)
                {
                    bool left_or_right = UtilityMethods.RandomBool();
                    if (left_or_right)
                        animator.SetTrigger("FallRight");
                    else
                        animator.SetTrigger("FallLeft");
                }

                StartCoroutine(UtilityMethods.WaitDoCR(() =>
                {
                    for (int i = 0; i < crop_detail.product_itemids.Length; i++)
                    {
                        int count = Random.Range(crop_detail.product_min_count[i], crop_detail.product_max_count[i]);
                        for (int c = 0; c < count; c++)
                        {
                            Vector2 delta_pos = new Vector2(Random.Range(-crop_detail.spawn_radius, crop_detail.spawn_radius),
                                                            Random.Range(-crop_detail.spawn_radius, crop_detail.spawn_radius));
                            Vector2 center_pos = (Vector2)transform.position;
                            ItemObject product = WorldItemManager.instance.MakeItem(crop_detail.product_itemids[i], center_pos);
                            product.Move(center_pos + delta_pos);
                        }
                    }
                    harvest_action_count[tool_index] = 0;
                    UpdateStatus(-3);
                },1));
            }
        }

    }
}
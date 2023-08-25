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
        //NOTE 子类有则覆盖基类，不会主动调用基类，除非base。子类没有则会自动调用基类，即使基类是private
        protected override void Awake()
        {
            animator=GetComponentInChildren<Animator>();
            base.Awake();
        }
        /// <summary>
        /// 根据current_day 更新当前植物的生长状态
        /// </summary>
        protected override void HarvestComplete()
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
                UpdateStatus(-3);
            }, 1));
        }

        protected override void HarvestAnimation()
        {
            Debug.Log("son anima");

            bool left_or_right = UtilityMethods.RandomBool();
            if (left_or_right)
                animator.SetTrigger("RotateRight");
            else
                animator.SetTrigger("RotateLeft");
        }

    }
}
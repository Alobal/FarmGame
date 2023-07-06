using Crop;
using Item;
using Map;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crop
{
    [RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
    public class CropObject : MonoBehaviour
    {
        public int seed_id = 0;//用于Start中根据id找到cropdetail
        public Vector2Int tile_pos;//所属tile的grid pos
        private CropDetail crop_detail;
        private SpriteRenderer sprite_render;//item渲染图片
        private BoxCollider2D collide;
        [SerializeField] private int current_day = 0;//当前种下的天数

        public int grow_stage//当前处于的生长阶段
        {
            get
            {
                if (crop_detail == null)
                    throw new NullReferenceException("Crop Detail is Null");
                return crop_detail.ComputeGrowStage(current_day);
            }
        }  
        private void Start()
        {
            sprite_render = GetComponent<SpriteRenderer>();
            collide = GetComponent<BoxCollider2D>();
            if (seed_id != 0)//存在预设id
                Init(seed_id);//TODO NOTE 不能在Awake里调用Awake唤醒的单例
        }

        private void OnEnable()
        {
            TimeManager.DayEvent += OnDayEvent;
        }

        private void OnDisable()
        {
            TimeManager.DayEvent -= OnDayEvent;
        }

        private void OnDayEvent(int delta_day)
        {
            current_day += delta_day;
            UpdateStatus();
        }

        /// <summary>
        /// 根据current_day 更新当前植物的生长状态
        /// </summary>
        private void UpdateStatus()
        {
            int current_stage = grow_stage;
            if(current_stage<crop_detail.grow_days.Length)//小于则处于生长阶段
                sprite_render.sprite = crop_detail.sprites[current_stage];
            else//成熟阶段
            {

            }
        }

        /// <summary>
        /// 根据crop detail和生长天数，初始化相关状态。
        /// </summary>
        /// <param name="init_id"></param>
        /// <param name="init_day"></param>
        private void Init(int init_id, int init_day = 0)
        {

            crop_detail = CropManager.instance.GetCropDetail(init_id);
            current_day = init_day;
            UpdateStatus();
        }

        public SaveData GetSaveData()
        {
            return new SaveData()
            {
                seed_id = this.seed_id,
                current_day = this.current_day,
                tilepos_x=tile_pos.x,
                tilepos_y=tile_pos.y,
                pos_x = transform.position.x,
                pos_y = transform.position.y,
            };
        }

        [Serializable]
        public struct SaveData
        {
            public int seed_id;
            public int current_day;
            public int tilepos_x,tilepos_y;
            public float pos_x, pos_y;
        }
    }
}
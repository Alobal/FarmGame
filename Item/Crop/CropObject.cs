using Audio;
using Crop;
using Item;
using Map;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
namespace Crop
{
    [RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
    public class CropObject : MonoBehaviour
    {
        public int seed_id = 0;//用于Start中根据id找到cropdetail
        public Vector2Int tile_pos;//所属tile的grid pos
        public int current_day = 0;//当前种下的天数
        protected CropDetail crop_detail;
        protected int[] harvest_action_count;
        //Component
        public GameObject prefab_now;//用于记录当前prefab，并根据阶段切换。
        protected SpriteRenderer sprite_render;//item渲染图片
        protected BoxCollider2D collide;

        [Header("Effect")]
        protected Animator animator;
        [SerializeField] protected GameObject particle;
        [SerializeField] protected Vector2 particle_localpos;
        protected Vector2 particle_pos;
        [SerializeField] protected SoundName harvest_sound_name;

        public int grow_stage//当前处于的生长阶段
        {
            get
            {
                if (crop_detail == null)
                    throw new NullReferenceException("Crop Detail is Null");
                return crop_detail.ComputeGrowStage(current_day);
            }
        }
        public bool is_wild { get { return tile_pos==Vector2.zero; } }//是野生则仅在开始游戏时加载
        public bool is_ripe { get { return grow_stage >= crop_detail.grow_days.Length; } }

        protected virtual void Start()
        {
            sprite_render = GetComponent<SpriteRenderer>();
            collide = GetComponent<BoxCollider2D>();
            particle_pos = (Vector2)transform.position + particle_localpos;
            if (seed_id != 0)//外部设置好seed后 自动内部初始化
                InitInternal();//NOTE 不能在Awake里调用Awake唤醒的单例
        }

        private void OnEnable()
        {
            TimeManager.DayEvent += OnDayEvent;
        }

        private void OnDisable()
        {
            TimeManager.DayEvent -= OnDayEvent;
        }

        public bool CheckHarvestable(int  tool_id)
        {
            int tool_index = crop_detail.FindHarvestTool(tool_id);
            if (tool_index == -1 || !is_ripe) 
                return false;
            return true;
        }

        /// <summary>
        /// 执行一次收割动作。一个作物可能需要收割几次。
        /// </summary>
        /// <param name="tool_id"></param>
        public virtual void HarvestOnce(int tool_id)
        {
            Debug.Log("is haravesting...");
            //单次收割
            CheckHarvestable(tool_id);
            int tool_index=crop_detail.FindHarvestTool(tool_id);
            harvest_action_count[tool_index] += 1;
            if (harvest_sound_name != SoundName.None)
                HarvestSound();
            //播放动画
            if (animator != null)
                HarvestAnimation();
            //播放粒子
            if (particle != null)
            {
                ParticleSystem particle_object = ObjectPoolManager.instance.Get(particle, particle_pos, 1.0f)
                                                .GetComponent<ParticleSystem>();
            }
            //收割次数满足 实现收获
            if (harvest_action_count[tool_index] >= crop_detail.require_harvest_actions[tool_index])
            {
                HarvestComplete();
                harvest_action_count[tool_index] = 0;
            }
        }

        protected virtual void HarvestComplete()
        {
            for (int i = 0; i < crop_detail.product_itemids.Length; i++)
            {
                int count = Random.Range(crop_detail.product_min_count[i], crop_detail.product_max_count[i] + 1);
                for (int c = 0; c < count; c++)
                {
                    Vector2 delta_pos = new Vector2(Random.Range(-crop_detail.spawn_radius, crop_detail.spawn_radius + 1),
                                                    Random.Range(-crop_detail.spawn_radius, crop_detail.spawn_radius + 1));
                    Vector2 center_pos = (Vector2)transform.position;
                    ItemObject product = WorldItemManager.instance.MakeItem(crop_detail.product_itemids[i], center_pos);
                    product.Move(center_pos + delta_pos);
                }
            }
            UpdateStatus(-3);
        }

        protected virtual void HarvestSound()
        {
            ObjectPoolManager.instance.GetSound(harvest_sound_name);
        }
        protected virtual void HarvestAnimation()
        {
        }



        /// 根据crop detail和生长天数，初始化内部组件和状态。
        private void InitInternal()
        {

            crop_detail = CropManager.instance.GetCropDetail(seed_id);
            harvest_action_count = new int[crop_detail.harvest_toolids.Length];
            UpdateStatus(0);
        }

        private void OnDayEvent(int delta_day)
        {
            UpdateStatus(delta_day);
        }

        /// 根据时间变化 更新当前植物的生长状态
        protected void UpdateStatus(int delta_day)
        {
            current_day += delta_day;
            current_day = Math.Min(crop_detail.total_grow_days[^1], current_day);
            int current_stage = grow_stage;
            //更新prefab
            GameObject prefab_old = prefab_now;
            prefab_now = crop_detail.prefabs[current_stage];
            if (prefab_old != prefab_now)
                CropManager.instance.UpdateCropPrefab(this);
            //更新sprite
            sprite_render.sprite = crop_detail.sprites[current_stage];
            UtilityMethods.AdaptiveBoxColliderToSprite(sprite_render.sprite, collide);
            //更新碰撞体
            if (is_ripe)//成熟阶段 即最后一个阶段
            {
                collide.enabled = true;
            }
            else//生长阶段
            {
                collide.enabled = false;
            }
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
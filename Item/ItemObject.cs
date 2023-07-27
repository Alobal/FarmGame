using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Item
{
    /// <summary>
    /// 生成在世界中的ItemObject。
    /// </summary>
    public class ItemObject : MonoBehaviour
    {
        public int init_id;//用于初始化引导
        public int pack_index;//存储该Item在pack list中的索引

        public ItemDetail item_detail;//item详细结构信息
        private SpriteRenderer sprite_render;//item渲染图片
        public bool allow_pick = true;
        private BoxCollider2D collide;
        public int id
        {
            get { return item_detail.id; }
        }

        protected void Start()
        {
            sprite_render = GetComponent<SpriteRenderer>();
            collide = GetComponent<BoxCollider2D>();
            if (init_id != 0) 
                InitInternal(init_id);
        }
        /// <summary>
        /// 初始化数据信息，调整碰撞体尺寸
        /// </summary>
        /// <param name="item_id"></param>
        private void InitInternal(int item_id)
        {
            if (item_id == 0)
                return;
            item_detail = PackDataManager.instance.GetItemDetail(item_id);
            if (item_detail != null)
            {
                sprite_render.sprite = item_detail.world_sprite;
                Debug.Assert(sprite_render.sprite != null, $"{item_id} Item world sprite missing...");

                UtilityMethods.AdaptiveBoxColliderToSprite(sprite_render.sprite, collide);
            }
            if (item_detail.item_type == ItemType.Furniture)
            {
                allow_pick = false;
                GetComponent<BoxCollider2D>().isTrigger = false;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            //碰撞拾取
            if (collision.CompareTag("Player"))
            {
                if (item_detail.can_pick && allow_pick)
                    WorldItemManager.instance.PickUpItem(this);
            }
        }
        /// <summary>
        /// 协程移动
        /// </summary>
        /// <param name="target"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        /// 
        public void Move(Vector2 target, float duration = 0.3f)
        {
            StartCoroutine(MoveHelp(target,duration));
        }

        private IEnumerator MoveHelp(Vector2 target,float duration=0.3f)
        {
            allow_pick = false;
            float speed_y = (target.y - transform.position.y)/duration;
            float speed_x = (target.x - transform.position.x) / duration;
            Vector3 speed=new Vector3(speed_x,speed_y,0);
            while(Vector2.Distance(transform.position,target)>0.1f)
            {
                transform.position += speed * Time.deltaTime;
                yield return null;
            }
            allow_pick = true;
        }

        public SaveData GetSaveData()
        {
            return new SaveData()
            {
                init_id = this.id,
                pos_x = transform.position.x,
                pos_y = transform.position.y,
                pos_z = transform.position.z
            };
        }

        public void LoadSaveData(SaveData data)
        {
            init_id= data.init_id;
            transform.position = new Vector3(data.pos_x, data.pos_y, data.pos_z);
        }

        [Serializable]
        public struct SaveData
        {
            public int init_id;
            public float pos_x,pos_y,pos_z;
        }
    }
}

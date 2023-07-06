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
        [SerializeField] private int init_id;//用于编辑器给定初始化item
        public int pack_index;//存储该Item在pack list中的索引

        public ItemDetail item_detail;//item详细结构信息
        private SpriteRenderer sprite_render;//item渲染图片
        private bool allow_pick = true;
        private BoxCollider2D collide;
        public int id
        {
            get { return item_detail.id; }
        }

        protected void Awake()
        {
            sprite_render = GetComponent<SpriteRenderer>();
            collide = GetComponent<BoxCollider2D>();
            if (init_id != 0) 
                Init(init_id);
        }
        /// <summary>
        /// 初始化数据信息，调整碰撞体尺寸
        /// </summary>
        /// <param name="item_id"></param>
        public void Init(int item_id)
        {
            if (item_id == 0)
                return;
            item_detail = PackDataManager.instance.GetItemDetail(item_id);
            if (item_detail != null)
            {
                sprite_render.sprite = item_detail.world_sprite;
                Debug.Assert(sprite_render.sprite != null, $"{item_id} Item world sprite missing...");

                //自适应碰撞体尺寸
                Vector2 coll_size = sprite_render.sprite.bounds.size.XY();
                collide.size = coll_size;
                collide.offset = new Vector2(0, sprite_render.sprite.bounds.center.y);
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
        public IEnumerator Move(Vector2 target,float duration=0.3f)
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

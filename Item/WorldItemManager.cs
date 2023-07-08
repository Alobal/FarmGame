using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Item
{
    /// <summary>
    /// 场景world item 管理器，在运行时JSON序列化保存对每个scene内item的修改。
    /// </summary>
    public class WorldItemManager : Singleton<WorldItemManager>
    {
        public List<ItemObject> item_list;//开始游戏后持久化保存，退出游戏后清空
        public GameObject item_prefab;
        private const string serialize_file = "WorldItemList.json";
        private string serialize_dir;
        private string serialize_path;

        private void Start()
        {
            //收集手工预置的所有item
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).GetComponent<ItemObject>() is ItemObject world_item)
                    Add(world_item);
            }
            serialize_dir = $"{Application.dataPath}/Temp/{gameObject.scene.name}";
            serialize_path = $"{serialize_dir}/{serialize_file}";
            Load();
        }
        private void OnEnable()
        {
            TransitionManager.BeforeSceneUnload += Save;
        }

        private void OnDisable()
        {
            TransitionManager.BeforeSceneUnload -= Save;
        }

        private void OnApplicationQuit()
        {
            if (File.Exists(serialize_path))
                File.Delete(serialize_path);
        }
        /// <summary>
        /// 向玩家背包添加物品，地面销毁物品
        /// </summary>
        /// <param name="world_item"></param>
        public void PickUpItem(ItemObject world_item)
        {
            PackDataManager.instance.AddItemToPlayer(world_item.id);
            Remove(world_item);
        }

        private void Remove(ItemObject world_item)
        {
            item_list.RemoveAt(world_item.pack_index);
            Destroy(world_item.gameObject);
            //更新list index
            for (int i = world_item.pack_index; i < item_list.Count; i++)
                item_list[i].pack_index -= 1;
        }


        private void Add(ItemObject world_item)
        {
            world_item.pack_index = item_list.Count;
            item_list.Add(world_item);
        }

        public ItemObject MakeItem(int item_id, Vector3 pos)
        {
            var world_item = Instantiate(item_prefab, transform).GetComponent<ItemObject>();
            world_item.transform.position = pos;
            world_item.Init(item_id);
            Add(world_item);
            return world_item;
        }

        //如果存在记录list，则会删除scene上原有的item，重新生成记录的item
        private void Load()
        {
            if (File.Exists(serialize_path))
            {
                int count = item_list.Count;
                for (int i = 0; i < count; i++)
                    Remove(item_list[0]);

                string json_data = File.ReadAllText(serialize_path);
                var save_list = JsonUtility.FromJson<SaveData>(json_data).save_list;
                for (int i = 0; i < save_list.Count; i++)
                    MakeItem(save_list[i].init_id,
                             new Vector3(save_list[i].pos_x,
                             save_list[i].pos_y,
                             save_list[i].pos_z));
            }
        }

        private void Save()
        {
            if (!File.Exists(serialize_dir))
                Directory.CreateDirectory(serialize_dir);

            SaveData save_data = new();
            save_data.save_list = new();
            for (int i = 0; i < item_list.Count; i++)
                save_data.save_list.Add(item_list[i].GetSaveData());

            File.WriteAllText(serialize_path, JsonUtility.ToJson(save_data));
        }

        [InspectorButton("清除ItemList文件保存数据")]
        private void ClearSaveData()
        {
            if (File.Exists(serialize_path))
            {
                File.Delete(serialize_path);
                Debug.Log($"清除文件成功：{serialize_path}");
            }
            Debug.Log($"清除文件失败：{serialize_path}");
        }

        [Serializable]
        private struct SaveData
        {
            public List<ItemObject.SaveData> save_list;
        }
    }
}

using Save;
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
    public class WorldItemManager : Singleton<WorldItemManager>,Save.ISavable
    {
        public List<ItemObject> item_list;//开始游戏后持久化保存，退出游戏后清空
        public GameObject item_prefab;
        public GameObject building_item_prefab;
        private const string serialize_file = "WorldItemList.json";
        private string serialize_dir;
        private string serialize_path;//临时存储文件，以便让有些东西在游戏开始后临时保存，游戏退出后复原。

        string ISavable.GUID => GetComponent<Save.Guid>().guid;

        protected override void Awake()
        {
            base.Awake();

            //收集手工预置的所有item
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).GetComponent<ItemObject>() is ItemObject world_item)
                    AddItemToList(world_item);

            }
            serialize_dir = $"{Application.dataPath}/Temp/{gameObject.scene.name}";
            serialize_path = $"{serialize_dir}/{serialize_file}";
            TransitionLoad();

            //注册为保存对象
            ISavable savable = this;
            savable.RegisterSaveObject();

        }

        private void Start()
        {

        }
        private void OnEnable()
        {
            TransitionManager.BeforeSceneUnload += TransitionSave;
        }

        private void OnDisable()
        {
            TransitionManager.BeforeSceneUnload -= TransitionSave;
        }

        private void OnApplicationQuit()
        {
            if (File.Exists(serialize_path))
            {
                File.Delete(serialize_path);
                Debug.Log("清除临时文件成功");
            }
        }
        /// <summary>
        /// 向玩家背包添加物品，地面销毁物品
        /// </summary>
        /// <param name="world_item"></param>
        public void PickUpItem(ItemObject world_item)
        {
            ObjectPoolManager.instance.GetSound(Audio.SoundName.Pickup);
            PackDataManager.instance.PlayerAddItem(world_item.init_id);
            Remove(world_item);
        }

        public void Remove(ItemObject world_item)
        {
            item_list.RemoveAt(world_item.pack_index);
            Destroy(world_item.gameObject);
            //更新list index
            for (int i = world_item.pack_index; i < item_list.Count; i++)
                item_list[i].pack_index -= 1;
        }


        private void AddItemToList(ItemObject world_item)
        {
            world_item.pack_index = item_list.Count;
            item_list.Add(world_item);
        }

        public ItemObject MakeItem(int item_id, Vector3 pos)
        {
            var world_item = Instantiate(item_prefab, transform).GetComponent<ItemObject>();
            world_item.transform.position = pos;
            world_item.init_id=item_id;
            AddItemToList(world_item);
            return world_item;
        }

        public BuildingItem MakeBuildingItem(int item_id, Vector3 pos, SlotItem[] need_resource=null)
        {
            var building_item = Instantiate(building_item_prefab, transform).GetComponent<BuildingItem>();
            building_item.transform.position = pos;
            building_item.init_id = item_id;
            if(need_resource!=null)
                building_item.need_resource = need_resource;
            AddItemToList(building_item);
            return building_item;
        }

        //如果存在记录文件，则会删除scene上原有的item，重新生成记录的item
        private void TransitionLoad()
        {
            if (File.Exists(serialize_path))
            {
                int count = item_list.Count;
                for (int i = 0; i < count; i++)
                    Remove(item_list[0]);

                string json_data = File.ReadAllText(serialize_path);
                var save_list = JsonUtility.FromJson<SaveData>(json_data).item_list;
                for (int i = 0; i < save_list.Count; i++)
                    MakeItem(save_list[i].init_id,
                             new Vector3(save_list[i].pos_x,
                             save_list[i].pos_y,
                             save_list[i].pos_z));
            }
        }

        //保存功能交给场景转换器管理，而不是生命周期事件管理，因为希望每次重开游戏时刷新这些数据，但是场景转移时保留数据更改。
        private void TransitionSave()
        {
            if (!File.Exists(serialize_dir))
                Directory.CreateDirectory(serialize_dir);

            SaveData save_data =GetSaveData();

            File.WriteAllText(serialize_path, JsonUtility.ToJson(save_data));
        }

        public SaveData GetSaveData()
        {
            SaveData save_data = new()
            {
                item_list = new(),
                building_list = new()
            };
            for (int i = 0; i < item_list.Count; i++)
                switch (item_list[i])
                {
                    case BuildingItem:
                        save_data.building_list.Add(((BuildingItem)item_list[i]).GetSaveData());
                        break;
                    case ItemObject :
                        save_data.item_list.Add(item_list[i].GetSaveData());
                        break;
                }
            return save_data;
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

        public void SaveProfile()
        {
            var save_data = GetSaveData();
            GameSaveData.instance.scene_items[gameObject.scene.name]=save_data;
        }

        public  void LoadProfile()
        {
            if(GameSaveData.instance.scene_items[gameObject.scene.name] is SaveData save_data)
            {
                int count = item_list.Count;
                for (int i = 0; i < count; i++)
                    Remove(item_list[0]);

                var items = save_data.item_list;
                for (int i = 0; i < items.Count; i++)
                    MakeItem(items[i].init_id,
                             new Vector3(items[i].pos_x,
                             items[i].pos_y,
                             items[i].pos_z));

                var builds = save_data.building_list;
                for (int i = 0; i < builds.Count; i++)
                    MakeBuildingItem(builds[i].blueprint_id,
                                     builds[i].pos,
                                     builds[i].need_resource);
            }
        }

        [Serializable]
        public struct SaveData
        {
            public List<ItemObject.SaveData> item_list;
            public List<BuildingItem.SaveData> building_list;
        }
    }
}

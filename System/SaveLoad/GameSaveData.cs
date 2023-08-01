using Crop;
using Item;
using Map;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace Save
{
    [Serializable]
    public class GameSaveData
    {
        private static GameSaveData m_instance;
        public static GameSaveData instance
        {
            get
            {
                if(m_instance == null)
                    m_instance = new GameSaveData();
                return m_instance;
            }
            set
            {
                m_instance = value;
            }
        }
        public string scene_name;
        public Dictionary<string,SerialVec3> character_pos=new();//各角色位置
        public Dictionary<string, WorldItemManager.SaveData> scene_items = new();
        public Dictionary<string, List<Vector2Int>> scene_tile_pos = new();//NOTE 字典的复杂键值会序列化为string，并且难以反序列化。
        public Dictionary<string, List<TileDetail>> scene_tile_detail = new();//NOTE 字典的复杂键值会序列化为string，并且难以反序列化。
        public Dictionary<string, CropManager.SaveData> scene_crops = new();
        public Dictionary<string, ItemPackSO> pack_data = new();//背包数据
        [JsonIgnore]public Dictionary<string,ISavable> save_objects = new();
        public int second_total;//逻辑时间

        public void Save()
        {
            foreach (var o in save_objects)
            {
                o.Value.Save();
            }
        }

        public void Load()
        {
            foreach (var o in save_objects)
            {
                o.Value.Load();
            }
        }

        public static void LoadFromJson(string json_data)
        {
            //临时保存save_objects 以防json的空对象覆盖
            var save_objects = GameSaveData.instance.save_objects;
            GameSaveData.instance = JsonConvert.DeserializeObject<GameSaveData>(json_data);
            GameSaveData.instance.save_objects = save_objects;
        }
    }

    public interface ISavable
    {
        string GUID { get; }
        public void RegisterSaveObject()
        {

            ISavable savable = this;
            GameSaveData.instance.save_objects[GUID]=savable;
        }

        public void Save();
        public void Load();

    }

}
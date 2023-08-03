using Crop;
using Item;
using Map;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

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
        public DateTime save_time;

        public GameSaveData()
        {
            TransitionManager.BeforeSceneUnload += BeforeSceneUnload;
        }
        public void Save()
        {
            foreach (var o in save_objects)
            {
                o.Value.Save();
            }
            scene_name = SceneManager.GetActiveScene().name;
            save_time= DateTime.Now.ToLocalTime();
        }

        /// <summary>
        /// 加载流程：当前场景卸载 => BeforeSceneUnload事件 => 新场景的Awake和OnEnable => AfterSceneLoad
        /// </summary>
        public void Load()
        {
            //加载存档场景
            string active_scene_name = SceneManager.GetActiveScene().name;
            //如果加载前处于持久场景，则不卸载该场景。
            if (active_scene_name == "MainScene" || active_scene_name == "UIScene")
                active_scene_name = string.Empty;
            else//否则则卸载当前场景
                save_objects.Clear();
            TransitionManager.AfterSceneLoad += LoadObject;
            TransitionManager.instance.StartTransition(active_scene_name, scene_name);


        }

        private void BeforeSceneUnload()
        {
            save_objects.Clear();
        }
        //在读取存档加载场景之后调用，对场景中的对象进行加载操作。
        private void LoadObject(object sender, AfterSceneLoadEventArgs e)
        {
            foreach (var o in save_objects)
            {
                o.Value.Load();
            }
            TransitionManager.AfterSceneLoad -= LoadObject;
        }

        public static void LoadFromJson(string json_data)
        {
            //临时保存save_objects 以防json的空对象覆盖
            var save_objects = instance.save_objects;
            instance = JsonConvert.DeserializeObject<GameSaveData>(json_data);
            instance.save_objects = save_objects;
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
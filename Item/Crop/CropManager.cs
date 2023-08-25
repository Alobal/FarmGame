using Item;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Map;
using Save;

namespace Crop
{
    public class CropManager : Singleton<CropManager>,Save.ISavable
    {
        public CropSourceDataSO source_data;
        public List<CropObject> crop_list;//在tile中种植的crop，需要在退出时保存所有状态，并在开始时重新加载
        private const string crop_save_file = "CropList.json";//运行时存储文件，退出游戏清空
        private string save_dir;
        private string crop_save_path;

        public string GUID => GetComponent<Save.Guid>().guid;

        private new void Awake()
        {
            base.Awake();
            save_dir = $"{Application.dataPath}/Temp/{gameObject.scene.name}";
            crop_save_path = $"{save_dir}/{crop_save_file}";

            //注册为保存对象
            ISavable savable = this;
            savable.RegisterSaveObject();
        }
        private void OnEnable()
        {
            ReadFile();
            TransitionManager.BeforeSceneUnload +=SaveOnTransition;
        }

        private void Start()
        {

        }


        private void OnDisable()
        {
            TransitionManager.BeforeSceneUnload -=SaveOnTransition;
        }

        private void OnApplicationQuit()
        {
            //清除运行时存储文件
            if (File.Exists(crop_save_path))
            {
                File.Delete(crop_save_path);
            }
        }


        //替换Crop的prefab，例如作物转变成大树
        public void UpdateCropPrefab(CropObject source)
        {
            var data = source.GetSaveData();
            RemoveCrop(source);
            MakeCrop(data);
        }
        public CropDetail GetCropDetail(int seed_id)
        {
            return source_data.crop_details.Find(x => x.seed_id == seed_id);
        }

        public void MakeCrop(CropObject.SaveData data)
        {
            MakeCrop(data.seed_id,
                     new Vector2(data.pos_x, data.pos_y),
                     new Vector2Int(data.tilepos_x, data.tilepos_y),
                     data.current_day);
        }

        /// 实例化Crop，并设置好相关外部数据。
        public void MakeCrop(int seed_id,Vector2 pos,Vector2Int tile_pos,int init_day=0)
        {
            //FIX：如何判断场景是否第一次加载。这里用坐标避免重复来取巧，避免重复加载场景时，重复加载预设的野生crop
            if (GetCropObject(pos) != null)
                return;

            CropDetail crop_detail=GetCropDetail(seed_id);
            GameObject init_prefab=source_data.GetPrefab(seed_id, init_day);
            //if (init_prefab == null)
            //    init_prefab = crop_detail.prefabs[0];
            CropObject crop_object = Instantiate(init_prefab,transform).GetComponent<CropObject>();

            crop_object.transform.position = pos;
            crop_object.prefab_now=init_prefab;
            crop_object.seed_id = seed_id;
            crop_object.tile_pos = tile_pos;
            crop_object.current_day = init_day;
            crop_list.Add(crop_object);
        }

        /// <summary>
        /// 销毁CropObject，默认也会从维护列表crop list中移除。
        /// </summary>
        /// <param name="crop_object"></param>
        /// <param name="remove_in_list">提供是否从维护列表crop list中移除的选项，避免在循环中调用Remove</param>
        public void RemoveCrop(CropObject crop_object,bool remove_in_list=true)
        {
            if(TilemapManager.instance.GetTileDetail(crop_object.tile_pos) is TileDetail crop_tile)
                crop_tile.seeded=false; 
            Destroy(crop_object.gameObject);
            if(remove_in_list)
            {
                crop_list.Remove(crop_object);
            }
        }

        /// <summary>
        /// 清空地图上所有的Crop。
        /// </summary>
        /// <param name="clear_wild">是否清空预置的wild crop</param>
        public void Clear(bool clear_wild=false)
        {
            for (int i = crop_list.Count-1; i >=0 ; i--)
                if (clear_wild == false && crop_list[i].is_wild)
                    continue;
                else
                    RemoveCrop(crop_list[i], true);
        }

        private CropObject GetCropObject(Vector2 pos)
        {
            return crop_list.Find(x => (Vector2) x.transform.position == pos);
        }
        #region 运行时存储
        //读取记录时，清空Manager已管理的crop，再从Once文件读取生成。
        private void ReadFile()
        {
            if (File.Exists(crop_save_path))
            {
                for (int i = 0; i < crop_list.Count; i++)
                    RemoveCrop(crop_list[i],false);
                crop_list.Clear();
                //读取文件 场景中实例化数据
                string json_data = File.ReadAllText(crop_save_path);
                var save_list = JsonUtility.FromJson<SaveData>(json_data).save_list;
                for (int i = 0; i < save_list.Count; i++)
                    MakeCrop(save_list[i].seed_id,
                             new Vector2(save_list[i].pos_x,save_list[i].pos_y),
                             new Vector2Int(save_list[i].tilepos_x, save_list[i].tilepos_y),
                             save_list[i].current_day);
            }
        }

        /// <summary>
        /// 保存场景所有作物数据。
        /// 针对于wild crop，仅在运行切换场景时会保存，结束游戏则会清空wild crop存储的数据，
        /// 以便下次游戏开始时直接从场景预设读取野生Crop。
        /// </summary>
        /// <param name="save_wild">fasle时不保存野生Crop状态。在游戏退出时False，</param>
        private void WriteFile(bool save_wild=true)
        {
            if (!File.Exists(save_dir))
                Directory.CreateDirectory(save_dir);

            SaveData save_data = GetSaveData(save_wild);

            File.WriteAllText(crop_save_path, JsonUtility.ToJson(save_data));
        }

        #endregion


        private void SaveOnTransition() 
        { 
            WriteFile(); 
        }

#if UNITY_EDITOR
        [InspectorButton("清除Crop数据")]
        private void ClearSaveData()
        {
            for (int i = 0; i < crop_list.Count; i++)
                RemoveCrop(crop_list[i], false);
            crop_list.Clear();

            if (File.Exists(crop_save_path))
            {
                File.Delete(crop_save_path);
                Debug.Log($"清除文件成功：{crop_save_path}");
            }
            else
                Debug.Log($"清除文件失败：{crop_save_path}");
        }
#endif
        #region 存档相关
        public void SaveProfile()
        {
            SaveData save_data = GetSaveData(true);
            GameSaveData.instance.scene_crops[gameObject.scene.name]=save_data;
        }

        public void LoadProfile()
        {
            if(GameSaveData.instance.scene_crops[gameObject.scene.name] is SaveData save_data)
            {
                for (int i = 0; i < crop_list.Count; i++)
                    RemoveCrop(crop_list[i], false);
                crop_list.Clear();

                var save_list = save_data.save_list;
                for (int i = 0; i < save_list.Count; i++)
                    MakeCrop(save_list[i].seed_id,
                             new Vector2(save_list[i].pos_x, save_list[i].pos_y),
                             new Vector2Int(save_list[i].tilepos_x, save_list[i].tilepos_y),
                             save_list[i].current_day);
            }
        }
        #endregion

        [Serializable]
        public struct SaveData
        {
            public List<CropObject.SaveData> save_list;
        }


        public SaveData GetSaveData(bool save_wild)
        {
            SaveData save_data = new();
            save_data.save_list = new();
            for (int i = 0; i < crop_list.Count; i++)
            {
                if (crop_list[i].is_wild && save_wild == false)//野生作物特殊处理
                    continue;
                save_data.save_list.Add(crop_list[i].GetSaveData());
            }
            return save_data;
        }
    }
}
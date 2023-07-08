using Item;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Map;

namespace Crop
{
    public class CropManager : Singleton<CropManager>
    {
        public CropSourceDataSO source_data;
        public List<CropObject> crop_list;//地图上现存的crop
        private const string serialize_file = "CurrentCropList.json";
        private string serialize_dir;
        private string serialize_path;


        private new void Awake()
        {
            base.Awake();
            serialize_dir = $"{Application.dataPath}/Temp/{gameObject.scene.name}";
            serialize_path = $"{serialize_dir}/{serialize_file}";
            
        }

        private void OnEnable()
        {
            Load(); 
        }

        private void OnDisable()
        {
            Save();
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
                     data.current_day,
                     data.prefab);
        }

        /// 实例化Crop，并设置好相关外部数据。
        public void MakeCrop(int seed_id,Vector2 pos,Vector2Int tile_pos,int init_day=0,GameObject init_prefab=null)
        {
            CropDetail crop_detail=GetCropDetail(seed_id);
            if (init_prefab == null)
                init_prefab = crop_detail.prefabs[0];
            CropObject crop_object = Instantiate(init_prefab,transform).GetComponent<CropObject>();

            crop_object.transform.position = pos;
            crop_object.prefab_now=init_prefab;
            crop_object.seed_id = seed_id;
            crop_object.tile_pos = tile_pos;
            crop_object.current_day = init_day;
            crop_list.Add(crop_object);
        }

        //移除Tile中的Crop，注意不会从crop list中移除，以便循环使用
        public void RemoveCrop(CropObject crop_object,bool remove_in_list=true)
        {
            TilemapManager.instance.GetTileDetail(crop_object.tile_pos).seeded = false;
            Destroy(crop_object.gameObject);
            if(remove_in_list)
            {
                crop_list.Remove(crop_object);
            }
        }

        //读取记录时，清空场景上原有的crop，再从记录文件读取生成。
        private void Load()
        {
            if (File.Exists(serialize_path))
            {
                for (int i = 0; i < crop_list.Count; i++)
                    RemoveCrop(crop_list[i],false);
                crop_list.Clear();
                //读取文件 场景中实例化数据
                string json_data = File.ReadAllText(serialize_path);
                var save_list = JsonUtility.FromJson<SaveData>(json_data).save_list;
                for (int i = 0; i < save_list.Count; i++)
                    MakeCrop(save_list[i].seed_id,
                             new Vector2(save_list[i].pos_x,save_list[i].pos_y),
                             new Vector2Int(save_list[i].tilepos_x, save_list[i].tilepos_y),
                             save_list[i].current_day,
                             save_list[i].prefab);
            }
        }

        private void Save()
        {
            if (!File.Exists(serialize_dir))
                Directory.CreateDirectory(serialize_dir);

            SaveData save_data = new();
            save_data.save_list = new();
            for (int i = 0; i < crop_list.Count; i++)
                save_data.save_list.Add(crop_list[i].GetSaveData());

            File.WriteAllText(serialize_path, JsonUtility.ToJson(save_data));
        }

        [InspectorButton("清除Crop数据")]
        private void ClearSaveData()
        {
            for (int i = 0; i < crop_list.Count; i++)
                RemoveCrop(crop_list[i], false);
            crop_list.Clear();

            if (File.Exists(serialize_path))
            {
                File.Delete(serialize_path);
                Debug.Log($"清除文件成功：{serialize_path}");
            }
            else
                Debug.Log($"清除文件失败：{serialize_path}");
        }

        [Serializable]
        private struct SaveData
        {
            public List<CropObject.SaveData> save_list;
        }
    }
}
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


        public CropDetail GetCropDetail(int seed_id)
        {
            return source_data.crop_details.Find(x => x.seed_id == seed_id);
        }

        /// <summary>
        /// 加载crop detail，生成prefab
        /// </summary>
        /// <param name="seed_id"></param>
        /// <param name="pos"></param>
        /// <param name="init_day"></param>
        public void MakeCrop(int seed_id,Vector2 pos,Vector2Int tile_pos,int init_day=0)
        {
            CropDetail crop_detail=GetCropDetail(seed_id);
            var crop_object = Instantiate(crop_detail.prefabs[0],transform).GetComponent<CropObject>();
            crop_object.transform.position = pos;
            crop_object.seed_id = seed_id;
            crop_object.tile_pos = tile_pos;
            crop_list.Add(crop_object);
        }

        public void RemoveCrop(CropObject crop_object)
        {
            TilemapManager.instance.GetTileDetail(crop_object.tile_pos).seeded = false;
            Destroy(crop_object.gameObject);
        }

        //读取记录时，清空场景上原有的crop，再从记录文件读取生成。
        private void Load()
        {
            if (File.Exists(serialize_path))
            {
                int count = crop_list.Count;
                for (int i = 0; i < count; i++)
                {
                    Destroy(crop_list[i].gameObject);
                    crop_list.RemoveAt(i);
                };

                string json_data = File.ReadAllText(serialize_path);
                var save_list = JsonUtility.FromJson<SaveData>(json_data).save_list;
                for (int i = 0; i < save_list.Count; i++)
                    MakeCrop(save_list[i].seed_id,
                             new Vector2(save_list[i].pos_x,save_list[i].pos_y),
                             new Vector2Int(save_list[i].tilepos_x, save_list[i].tilepos_y),
                             save_list[i].current_day);
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
                RemoveCrop(crop_list[i]);
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
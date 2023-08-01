using Newtonsoft.Json;
using Save;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Save
{
    public class SaveLoadManager : MonoBehaviour
    {

        private string save_dir;
        // Start is called before the first frame update
        void Start()
        {
            save_dir = $"{Application.dataPath}/GameData/";

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                Save(0);
            }

            if (Input.GetKeyDown(KeyCode.F6))
            {
                Load(0);
            }
        }

        public void Save(int index)
        {
            string file_name = $"save_{index}.json";
            string save_path = save_dir + file_name;
            GameSaveData.instance.Save();
            if (!Directory.Exists(save_dir))
                Directory.CreateDirectory(save_dir);

            var serialize_data = JsonConvert.SerializeObject(GameSaveData.instance, Formatting.Indented);
            File.WriteAllText(save_path, serialize_data);
        }

        public void Load(int index)
        {
            string file_name = $"save_{index}.json";
            string save_path = save_dir + file_name;
            if (!File.Exists(save_path))
                Debug.Log("存档不存在");

            string serialize_data = File.ReadAllText(save_path);
            GameSaveData.LoadFromJson(serialize_data);
            GameSaveData.instance.Load();
        }
    }
}
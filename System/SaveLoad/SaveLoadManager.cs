using Newtonsoft.Json;
using Save;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

namespace Save
{
    public class SaveLoadManager : MonoBehaviour
    {
        public static string save_dir { get; private set; }

        public static event Action NewGameEvent;//对持久场景的物体初始化，并且对离线数据初始化
        // Start is called before the first frame update
        void Start()
        {
            save_dir = $"{Application.dataPath}/GameData/";
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                Save(0);
            }
            else if (Input.GetKeyDown(KeyCode.F2))
            {
                Save(1);
            }
            else if (Input.GetKeyDown(KeyCode.F3))
            {
                Save(2);
            }
            else if (Input.GetKeyDown(KeyCode.F4))
            {
                Load(0);
            }
        }

        public static void StartNewGame()
        {

            TransitionManager.instance.StartTransition(null, "House1");
            NewGameEvent?.Invoke();

        }


        public static void Save(int index)
        {
            string file_name = $"save_{index}.json";
            string save_path = save_dir + file_name;
            GameSaveData.instance.Save();
            //将数据写入序列化文件
            if (!Directory.Exists(save_dir))
                Directory.CreateDirectory(save_dir);
            var serialize_data = JsonConvert.SerializeObject(GameSaveData.instance, Formatting.Indented);
            File.WriteAllText(save_path, serialize_data);
        }

        public static void Load(int index)
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
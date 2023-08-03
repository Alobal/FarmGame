using Save;
using System.IO;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class MenuUI : MonoBehaviour
{
    public GameObject[] persist_ui;//开始游戏后加载持久的UI
    public GameObject[] panels;//菜单各面板
    [SerializeField] private PlayableDirector intro_director;//开场动画控制
    //存档栏控制
    [SerializeField] private TextMeshProUGUI[] slot_show_time;
    [SerializeField] private TextMeshProUGUI[] slot_show_scene;
    private bool[] slot_valid=new bool[3];

    private void OnEnable()
    {
        //暂时关闭主UI
        foreach (var ui in persist_ui)
        {
            ui.SetActive(false);
        }
    }
    public void SwitchPanel(int index)
    {
        panels[index].transform.SetAsLastSibling();
        if (index == 2)
            ShowSaveSlot();
    }

    public void ShowSaveSlot()
    {
        for (int i = 0; i < 3; i++)
        {
            string file_name = $"save_{i}.json";
            string save_path = SaveLoadManager.save_dir + file_name;
            if (!File.Exists(save_path))
            {
                slot_show_time[i].text = "暂无存档";
                slot_show_scene[i].text = "点击开始新游戏";
                slot_valid[i] = false;
            }
            else
            {
                string serialize_data = File.ReadAllText(save_path);
                GameSaveData.LoadFromJson(serialize_data);
                slot_show_time[i].text = $"保存时间：{GameSaveData.instance.save_time}";
                slot_show_scene[i].text = $"最后场景：{GameSaveData.instance.scene_name}";
                slot_valid[i] = true;
            }
        }
    }
    public void LoadSave(int index)
    {
        gameObject.SetActive(false);
        SwitchPanel(0);//恢复首页置顶
        if (slot_valid[index])
        {
            SaveLoadManager.Load(index);
            //激活UI
            foreach (var ui in persist_ui)
            {
                ui.SetActive(true);
            }

        }
        else//开始新游戏
        {
            intro_director.gameObject.SetActive(true);
            intro_director.stopped += x =>
            {
                //激活UI
                foreach (var ui in persist_ui)
                {
                    ui.SetActive(true);
                }
                SaveLoadManager.StartNewGame();
            };
            intro_director.Play();
            
        }
    }

    public void ExitGame()
    {
        GameObject go = new ();
        Application.Quit();
        Debug.Log("退出");
    }
}

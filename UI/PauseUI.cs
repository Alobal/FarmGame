using Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    [SerializeField] private AudioMixer audio_mixer;
    [SerializeField] private Slider[] volume_sliders;
    [SerializeField] private MenuUI menu_ui;
    private string[] volume_name = new string[]{ "Master", "MusicMaster","Effect"};

    private void Start()
    {
        Time.timeScale = 0f;
        for(int i=0; i<volume_sliders.Length;i++)
        {
            volume_sliders[i].value = AudioManager.instance.GetMixerVolume(volume_name[i]);
        }
    }

    public void ChangeMasterValue(float value)
    {
        AudioManager.instance.SetMixerVolume(volume_name[0], value);
    }

    public void ChangeMusicValue(float value)
    {
        AudioManager.instance.SetMixerVolume(volume_name[1], value);
    }

    public void ChangeEffectValue(float value)
    {
        AudioManager.instance.SetMixerVolume(volume_name[2], value);
    }

    //返回游戏
    public void Continue()
    {
        gameObject.SetActive(false);//NOTE 即使是Destroy，也会执行完当前函数
        Time.timeScale = 1f;
    }

    public void ToMenu()
    {
        string current_scene = SceneManager.GetActiveScene().name;
        if (current_scene!= "MainScene" && current_scene != "UIScene")
        {
            SceneManager.UnloadSceneAsync(current_scene);
        }
        Time.timeScale = 1f;
        gameObject.SetActive(false);
        menu_ui.gameObject.SetActive(true);
    }
}

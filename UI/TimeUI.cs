using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeUI : MonoBehaviour
{
    [SerializeField] private RectTransform day_night_image;
    [SerializeField] private RectTransform clock_parent;
    [SerializeField] private Image season_image;
    [SerializeField] private TextMeshProUGUI date_text;
    [SerializeField] private TextMeshProUGUI time_text;
    [SerializeField] private Sprite[] season_sprites;
    private int current_daynight;
    private int season_i;
    private TimeManager time_manager;
    private List<Image> hour_blocks = new();
    private int block_nowi;//当前启用的clock_blocks的最大索引
    private float hour_per_block;//UI中每block代表时间
    private const int hour_per_daynight=Settings.hour_hold/4;//四昼夜切换时间

    private void Start()
    {
        //初始化成员
        time_manager = TimeManager.instance;
        hour_per_block=Settings.hour_hold/ clock_parent.childCount;
        //初始化block图片
        for(int i=0;i<clock_parent.childCount;i++)
        {
            hour_blocks.Add(clock_parent.GetChild(i).GetComponent<Image>());
        }
        OnHourEvent(0);
        OnDayEvent(0);
    }

    private void OnEnable()
    {
        TimeManager.HourEvent += OnHourEvent;
        TimeManager.DayEvent += OnDayEvent;
    }


    private void OnDisable()
    {
        TimeManager.HourEvent -= OnHourEvent;
        TimeManager.DayEvent -= OnDayEvent;
    }

    private void OnHourEvent(int delta_hour)
    {
        //更新时刻block
        UpdateBlock();
        //更新日夜
        UpdateDayNight();
    }
    private void OnDayEvent(int delta_day)
    {
        date_text.text= $"{time_manager.year_total}年{time_manager.month_current + 1}月{time_manager.day_current + 1}日";
        //更新季节
        if ((int)time_manager.season != season_i)
        {
            season_i = (int)time_manager.season;
            season_image.sprite = season_sprites[season_i];
        }
    }

    private void FixedUpdate()
    {
        //更新秒数
        time_text.text = $"{time_manager.hour_current}:{time_manager.minute_current}:{time_manager.second_current}";
    }



    /// <summary>
    /// 根据hour切换时间block显示
    /// </summary>
    private void UpdateBlock()
    {
        int block_index = (int)(time_manager.hour_current / hour_per_block);//block时间
        for (int i = 0; i < hour_blocks.Count; i++)
        {
            hour_blocks[i].enabled = i <= block_index ? true : false;
            hour_blocks[i].fillAmount = i < block_index ? 1 : 0;
        }
        hour_blocks[block_index].fillAmount = time_manager.hour_current % hour_per_block / hour_per_block;
    }

    private void UpdateDayNight()
    {
        int daynight_i = time_manager.hour_current / hour_per_daynight;
        if(daynight_i!= current_daynight)
        {
            Vector3 target_rotate = new(0, 0, 90*daynight_i-90);
            day_night_image.DORotate(target_rotate, 5);
            current_daynight = daynight_i;
        }

    }
}

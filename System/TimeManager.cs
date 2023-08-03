using Save;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏逻辑时间，代表作物作息时间等，非物理时间。
/// </summary>
public class TimeManager : Singleton<TimeManager>,Save.ISavable
{
    //共计时间
    public int second_total { get; private set; }
    public int minute_total { get; private set; }
    public int hour_total { get; private set; }
    public int day_total { get; private set; }
    public int month_total { get; private set; }
    public int year_total { get; private set; }

    //余数时间
    public int second_current { get => second_total % Settings.second_hold; }
    public int minute_current { get => minute_total % Settings.minute_hold; }
    public int hour_current { get => hour_total % Settings.hour_hold; }
    public int day_current { get => day_total % Settings.day_hold; }
    public int month_current { get => month_total % Settings.month_hold; }

    public int second_today { get => second_total-day_total*Settings.hour_hold*Settings.day_hold*Settings.minute_hold; }
    public Season season
    {
        get => (Season) (Settings.init_season + (month_total / Settings.season_hold)%Settings.season_hold);
    }

    public string GUID => GetComponent<Save.Guid>().guid;

    public bool clock_pause;

    public static Action<int> MinuteEvent;
    public static Action<int> HourEvent;
    public static Action<int> DayEvent;
    public static Action<int> MonthEvent;
    public static Action<int> YearEvent;

    //public event Action AfterDay;
    private float tik_time;

    private new void Awake()
    {
        base.Awake();
        minute_total = Settings.init_minute;
        hour_total = Settings.init_hour;
        day_total = Settings.init_day;
        month_total = Settings.init_month;
        year_total = Settings.init_year;

        //注册为保存对象
        ISavable savable = this;
        savable.RegisterSaveObject();
    }

    private void OnEnable()
    {
        TransitionManager.AfterSceneLoad += OnAfterSceneLoad;
    }

    private void OnDisable()
    {
        TransitionManager.AfterSceneLoad -= OnAfterSceneLoad;
    }

    private void OnAfterSceneLoad(object sender, AfterSceneLoadEventArgs e)
    {
        //注册为保存对象
        ISavable savable = this;
        savable.RegisterSaveObject();
    }

    private void FixedUpdate()
    {
        if(!clock_pause)
        {
            tik_time += Time.deltaTime*Settings.gametime_scale;
            if (Input.GetKeyDown(KeyCode.Tab))//调试加速
                tik_time += 3600*20;
            if(tik_time> 1)
            {

                second_total +=(int)tik_time;
                tik_time -= (int)tik_time;
                UpdateTimeEvent();
            }
        }
    }

    private void UpdateTimeEvent()
    {
        //delta minute
        int minutes_temp= Settings.init_minute+second_total / Settings.second_hold;
        int delta_min = minutes_temp - minute_total;
        if (delta_min>0)
        {
            minute_total = minutes_temp;
            MinuteEvent?.Invoke(delta_min);

            //continue hour
            int hour_temp = Settings.init_hour+second_total / (Settings.minute_hold * Settings.second_hold);
            int delta_hour = hour_temp - hour_total;
            if (delta_hour>0)
            {
                hour_total = hour_temp;
                HourEvent?.Invoke(delta_hour);

                //continue day
                int day_temp = Settings.init_day+second_total / (Settings.hour_hold * Settings.minute_hold *Settings.second_hold);
                int delta_day = day_temp - day_total;
                if (delta_day>0)
                {
                    day_total = day_temp;
                    DayEvent?.Invoke(delta_day);

                    //continue month
                    int month_temp = Settings.init_month+second_total / (Settings.day_hold * Settings.hour_hold
                                    * Settings.minute_hold * Settings.second_hold);
                    int delta_month = month_temp - month_total;
                    if (month_temp > month_total)
                    {
                        month_total = month_temp;
                        MonthEvent?.Invoke(delta_month);

                        //continue year
                        int year_temp = Settings.init_year + second_total / (Settings.month_hold * Settings.day_hold *
                                        Settings.hour_hold * Settings.minute_hold * Settings.second_hold);
                        int delta_year=year_temp - year_total;
                        if (year_temp > year_total)
                        {
                            year_total = year_temp;
                            YearEvent?.Invoke(delta_year);
                        }
                    }
                }
            }
        }
    }



    public static int ComputeSeconds(int hour,int minute)
    {
        return hour * Settings.hour_hold * Settings.minute_hold +
                minute * Settings.minute_hold;
    }

    public void Save()
    {
        GameSaveData.instance.second_total = second_total;
    }

    public void Load()
    {
        second_total=GameSaveData.instance.second_total;
        minute_total = Settings.init_minute;
        hour_total = Settings.init_hour;
        day_total = Settings.init_day;
        month_total = Settings.init_month;
        year_total = Settings.init_year;
        UpdateTimeEvent();

    }
}

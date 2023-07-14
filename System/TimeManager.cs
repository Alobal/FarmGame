using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏逻辑时间，代表作物作息时间等，非物理时间。
/// </summary>
public class TimeManager : Singleton<TimeManager>
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
    }

    private void FixedUpdate()
    {
        if(!clock_pause)
        {
            tik_time += Time.deltaTime*Settings.gametime_scale;
            if (Input.GetKeyDown(KeyCode.Tab))//调试加速
                tik_time += 3600*24;
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
        //minute
        int minutes_temp= second_total / Settings.second_hold;
        int delta_min = minutes_temp - minute_total;
        if (delta_min>0)
        {
            minute_total = minutes_temp;
            if(MinuteEvent!=null) MinuteEvent.Invoke(delta_min);

            //continue hour
            int hour_temp = second_total / (Settings.minute_hold * Settings.second_hold);
            int delta_hour = hour_temp - hour_total;
            if (delta_hour>0)
            {
                hour_total = hour_temp;
                if (HourEvent != null) HourEvent.Invoke(delta_hour);

                //continue day
                int day_temp = second_total / (Settings.hour_hold * Settings.minute_hold *Settings.second_hold);
                int delta_day = day_temp - day_total;
                if (delta_day>0)
                {
                    day_total = day_temp;
                    if (DayEvent != null) DayEvent.Invoke(delta_day);

                    //continue month
                    int month_temp =second_total / (Settings.day_hold * Settings.hour_hold
                                    * Settings.minute_hold * Settings.second_hold);
                    int delta_month = month_temp - month_total;
                    if (month_temp > month_total)
                    {
                        month_total = month_temp;
                        if (MonthEvent != null) MonthEvent.Invoke(delta_month);

                        //continue year
                        int year_temp = second_total / (Settings.month_hold * Settings.day_hold *
                                        Settings.hour_hold * Settings.minute_hold * Settings.second_hold);
                        int delta_year=year_temp - year_total;
                        if (year_temp > year_total)
                        {
                            year_total = year_temp;
                            if (YearEvent != null) YearEvent.Invoke(delta_year);
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

}

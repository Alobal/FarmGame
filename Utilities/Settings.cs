using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    //场景变化
    public const float fade_duration = 0.35f;
    public const float fade_alpha = 0.5f;
    //逻辑时间相关
    public const float gametime_scale = 5.0f;//多少倍物理时间更新一次逻辑时间
    public const int second_hold = 60;
    public const int minute_hold = 60;
    public const int hour_hold = 24;
    public const int day_hold = 30;
    public const int month_hold = 12;
    public const int season_hold = 3;
    //初始时间
    public const int init_minute = 0;
    public const int init_hour = 20;
    public const int init_day = 0;
    public const int init_month = 0;
    public const int init_year = 2023;
    public const int init_season = 0;

    //Player相关
    public const float throw_radius = 5.0f;
    

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Light
{


    [CreateAssetMenu(fileName = "LightConfigListSO", menuName = "Light/LightConfigListSO")]
    public class LightConfigListSO : ScriptableObject
    {
        public List<LightConfig> data;

        /// <summary>
        /// 寻找最接近对应hour的灯光配置
        /// </summary>
        /// <param name="hour"></param>
        /// <returns></returns>
        public LightConfig Get(int hour)
        {
            return data.FindLast(x => x.hour <= hour);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hour"></param>
        /// <returns></returns>
        public LightConfig GetSoft(int hour)
        {
            int i = data.FindLastIndex(x => x.hour <= hour);
            if (i == -1 || i == data.Count - 1)
                return data[^1];
            else
            {
                if (data[i].hour == hour)
                    return data[i];
                else//返回两个配置中间的插值
                    return LightConfig.Interpolation(data[i], data[i + 1], hour);
            }
        }
    }

    [Serializable]
    public class LightConfig
    {
        public Color color;
        public float intensity;
        public int hour;//开始作用的hour时间

        public static LightConfig Interpolation(LightConfig a, LightConfig b, float current_hour)
        {
            var result = new LightConfig();
            float bw = (current_hour - a.hour) / (b.hour - a.hour);
            float aw = 1 - bw;
            result.color = a.color * aw + b.color * bw;
            result.intensity = a.intensity * aw + b.intensity * bw;
            result.hour = (int)current_hour;
            return result;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

namespace Light
{
    public class LightControl : MonoBehaviour
    {
        [SerializeField] private LightConfigListSO config_list;
        private Light2D light_com;
        static float update_span = 1;//更新灯光时的过度时间
        bool is_updating = false;

        // Start is called before the first frame update
        void Awake()
        {
            light_com = GetComponent<Light2D>();
        }

        private void Start()
        {
            UpdateLight(0);
        }

        private void OnEnable()
        {
            TimeManager.HourEvent += UpdateLight;
        }

        private void OnDisable()
        {
            TimeManager.HourEvent -= UpdateLight;
        }

        private void UpdateLight(int delta_hour)
        {
            var target_config=config_list.GetSoft(TimeManager.instance.hour_current);
            if (target_config == null)
                return;
            //light_com.color=  target_config.color;
            //light_com.intensity= target_config.intensity;
            var start_config = new LightConfig() { color = light_com.color, intensity = light_com.intensity };
            StartCoroutine(UpdateLightCR(start_config, target_config));
        }

        private IEnumerator UpdateLightCR(LightConfig start_config,LightConfig target_config)
        {
            
            float cr_span = 0.05f;
            var wait_time = new WaitForSeconds(cr_span);
            while (is_updating)
            {
                yield return wait_time;
            }
            is_updating = true;

            int cr_count = (int)(LightControl.update_span / cr_span);
            Color diff_color=(target_config.color-start_config.color)/cr_count;
            float diff_intensity=(target_config.intensity-start_config.intensity)/cr_count;
            if (diff_color == Color.clear && diff_intensity == 0.0)
                yield break;
            for(int i=0;i< cr_count; i++)
            {
                light_com.color += diff_color;
                light_com.intensity += diff_intensity;
                yield return wait_time;
            }
            is_updating=false;
        }
    }
}
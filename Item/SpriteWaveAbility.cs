using Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteWaveAbility : MonoBehaviour
{
    private bool is_waving = false;
    private WaitForSeconds pause_time = new WaitForSeconds(0.04f);
    [SerializeField] private SoundName wave_sound_name;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (is_waving)
            return;
        if (collision.transform.position.x < transform.position.x)
        {
            StartCoroutine(Wave(false));//向右
        }
        else
        {
            StartCoroutine(Wave(true));//向左
        }
    }

    private IEnumerator Wave(bool to_left)
    {
        int wave_degree = to_left ? 2 : -2;
        is_waving = true;
        ObjectPoolManager.instance.GetSound(wave_sound_name);
        for(int i=0;i<4;i++)
        {
            transform.Rotate(0, 0, wave_degree);
            yield return pause_time;
        }

        for (int i = 0; i < 4; i++)
        {
            transform.Rotate(0, 0, -wave_degree);
            yield return pause_time;
        }

        is_waving= false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingUI : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup canvas_group;
    private bool is_fading=false;//fade锁

    private void Awake()
    {
        canvas_group=GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        TransitionManager.BeforeSceneUnload += FadeIn;
        TransitionManager.AfterSceneLoad += FadeOut;
    }

    private void OnDisable()
    {
        TransitionManager.BeforeSceneUnload -= FadeIn;
        TransitionManager.AfterSceneLoad -= FadeOut;
    }

    private void FadeIn()
    {
        StartCoroutine(ChangeCanvasAlpha(1f, 0.5f));
    }
    private void FadeOut(object sender,AfterSceneLoadEventArgs e)
    {
        StartCoroutine(ChangeCanvasAlpha( 0f, 0.5f));
    }

    private IEnumerator ChangeCanvasAlpha(float target,float duration=1f)
    {   //等待锁
        while (is_fading) yield return new WaitForSeconds(0.1f);
        is_fading=true;
        int iter_count = 20;
        float epoch_time = duration/iter_count;
        float step = (target - canvas_group.alpha) / iter_count;
        for(int i=-1;i<iter_count;i++) //more one iter 可以多，不能少
        {
            canvas_group.alpha += step;
            yield return new WaitForSeconds(epoch_time);
        }
        is_fading = false;

    }
}

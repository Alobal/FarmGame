using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteFadeAbility : MonoBehaviour
{
    private SpriteRenderer[] sprite_renders;
    private void Awake()
    {
        sprite_renders = GetComponentsInChildren<SpriteRenderer>();
    }
    /// <summary>
    /// 恢复color
    /// </summary>
    public void FadeIn()
    {
        Color target = new(1f, 1f, 1f, 1f);
        foreach (SpriteRenderer renderer in sprite_renders)
            if (renderer != null)
                renderer.DOColor(target, Settings.fade_duration);
    }

    /// <summary>
    /// 逐渐透明化
    /// </summary>
    public void FadeOut()
    {
        Color target = new(1f, 1f, 1f, Settings.fade_alpha);
        foreach (SpriteRenderer renderer in sprite_renders)
            if (renderer != null)
                renderer.DOColor(target, Settings.fade_duration);
    }
}

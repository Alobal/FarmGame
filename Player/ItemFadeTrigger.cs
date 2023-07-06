using Item;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFadeTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        SpriteFadeAbility[] fades =collision.GetComponentsInChildren<SpriteFadeAbility>();
        foreach (SpriteFadeAbility fade in fades)
            fade.FadeOut();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        SpriteFadeAbility[] fades = collision.GetComponentsInChildren<SpriteFadeAbility>();
        foreach (SpriteFadeAbility fade in fades)
            fade.FadeIn();
    }
}

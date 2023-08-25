using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotificationUI : Singleton<NotificationUI>
{
    private TextMeshProUGUI tmp;
    protected override void Awake()
    {
        base.Awake();
        tmp=transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        gameObject.SetActive(false);

    }
    public void Notify(string text)
    {
        tmp.text=text;
        gameObject.SetActive(true);
        StartCoroutine(UtilityMethods.WaitDoCR(() =>gameObject.SetActive(false), 1));
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class DialogueUI : Singleton<DialogueUI>
{

    public GameObject main_uibox;
    public Text content;
    public Image avatar_left, avatar_right;
    public Text name_left,name_right;
    public GameObject continue_box;

    
    private void Start()
    {
        continue_box.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    public void Invoke(DialoguePiece dialogue)
    {
        StartCoroutine(ShowDialogue(dialogue));
    }

    private IEnumerator ShowDialogue(DialoguePiece dialogue)
    {
        if(dialogue != null)
        {
            dialogue.is_done = false;
            main_uibox.SetActive(true);
            avatar_left.gameObject.SetActive(false) ;
            avatar_right.gameObject.SetActive(false);
            continue_box.SetActive(false);
            content.text = string.Empty;
            Image avatar= dialogue.is_left?avatar_left:avatar_right;
            Text name= dialogue.is_left ? name_left : name_right;
            //是否显示头像及名字
            if (dialogue.name!=string.Empty && dialogue.avatar!=null)
            {
                avatar.gameObject.SetActive(true);
                avatar.sprite = dialogue.avatar;
                name.text=dialogue.name;
            }
            else
            {
                avatar_right.gameObject.SetActive(false);
                avatar_left.gameObject.SetActive(false);
            }

            yield return content.DOText(dialogue.dialogue_text, 1f).WaitForCompletion();
            //结束一条对话
            dialogue.is_done=true;
            continue_box.SetActive(true) ;
        }
        else//无有效对话
        {
            main_uibox.SetActive(false);
            continue_box.SetActive(false);

        }
    }
}

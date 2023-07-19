
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DialoguePiece
{
    public Sprite avatar;
    public string name;
    public bool is_left;

    [TextArea] public string dialogue_text;
    public bool has_to_pause;
    public bool is_done;
    public UnityEvent AfterTalkEvent;
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(NPCMovement), typeof(BoxCollider2D))]
//每个NPC自己的对话控制器
public class DialogueController : MonoBehaviour
{
    //组件
    [SerializeField] private GameObject button_tip;
    private NPCMovement movement;
    //对话数据
    public UnityEvent FinishEvent;
    public List<DialoguePiece> dialogue_list = new();
    public bool is_talking;
    private bool player_near=false;
    private int diag_index;
    private bool can_talk { get { return movement.is_moving == false && !is_talking && player_near; } }
    private DialoguePiece current_diag{get { return dialogue_list[diag_index]; }}
    // Start is called before the first frame update
    void Start()
    {
        movement=GetComponent<NPCMovement>();
        diag_index = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        player_near = true;
        if (can_talk)
            button_tip.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)  
    {
        player_near = false;
        button_tip.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(can_talk && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(DialogueRoutine());
        }
    }

    //取对话的一条数据进行处理
    private IEnumerator DialogueRoutine()
    {
        if(diag_index < dialogue_list.Count)//剩余对话
        {
            movement.pause_moving = true;
            is_talking = true;
            //控制UI开始对话
            DialogueUI.instance.Invoke(current_diag);
            //等待这句话所有流程结束
            yield return new WaitUntil(() => current_diag.is_done);
            diag_index++;
            is_talking=false;
            movement.pause_moving = false;

        }
        else//到达对话末尾
        {
            DialogueUI.instance.Invoke(null);
            diag_index = 0;
            button_tip.SetActive(false);
            FinishEvent?.Invoke();
        }
    }
}

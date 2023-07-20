using Map;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(SpriteRenderer))]
public class NPCMovement : MonoBehaviour
{

    [Header("移动属性")]
    public float speed = 5;
    public bool is_moving;
    public ScheduleDataSO schedule_data;
    private Stack<AStar.Node> path_nodes;//每次为当前schedule生成的寻路节点
    private int schedule_index = 0;
    public bool pause_moving=false;//暂停移动，用于对话之类的

    //实时schedule属性
    private ScheduleDetail current_schedule 
    { get => schedule_index < schedule_data.count ? schedule_data[schedule_index] : null; }
    private Vector2Int current_gridpos 
    { get => (Vector2Int)grid.WorldToCell(transform.position); }
    private AnimationClip stop_animation//当前schedule到达后执行的动画
    { get => current_schedule != null ? current_schedule.clip_at_target : null; }

    //组件
    private Rigidbody2D rb;
    private SpriteRenderer sprite_renderer;
    private BoxCollider2D collide;
    private Animator animator;
    public AnimatorOverrideController override_controller;
    Grid grid;


    private void Start()
    {
        rb= GetComponent<Rigidbody2D>();
        sprite_renderer = GetComponent<SpriteRenderer>();
        collide = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        //注意override_controller不能嵌套创建，
        //假如animator.runtimeAnimatorController本身就是override_controller，则嵌套创建后会失去原runtime控制器的override数据。
        if (override_controller == null)
            override_controller = new(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = override_controller;
        grid = TilemapManager.instance.grid;
        path_nodes= new Stack<AStar.Node>();
        AlignPosToCell();
        //BuildPath(schedule_data.data[0]);
    }

    private void OnEnable()
    {
        TransitionManager.AfterSceneLoad += OnAfterSceneLoad;
    }


    private void OnDisable()
    {
        TransitionManager.AfterSceneLoad -= OnAfterSceneLoad;
    }

    private void FixedUpdate()
    {
        if (pause_moving==false && current_schedule != null)//存在可行的寻路目标
        {
            //还没有为current_schedule构建path，则尝试构建新的path
            if (path_nodes.Count == 0 && current_gridpos != current_schedule.target_gridpos)
            {
                BuildPath(current_schedule);
                animator.SetBool("play_event", false) ;
            }

            if (!is_moving)//当前不在移动，则要么去新的path节点，要么已经到达终点
            {
                if (path_nodes.Count > 0)//去新的path节点
                {
                    //设置动画
                    is_moving = true;
                    animator.SetBool("is_moving", is_moving);
                    //寻路到下一个节点
                    var new_node = path_nodes.Peek();
                    StartCoroutine(MoveCR(new_node));
                }
                else//到达path终点
                {
                    if (stop_animation != null)
                        override_controller["BlankAnimation"] = stop_animation;
                    animator.SetBool("is_moving", is_moving);//moving动画到终点才关
                    animator.SetBool("play_event", true);
                    //移动schedule指针
                    schedule_index += 1;
                    //是否循环schedule
                    if (schedule_data.loop && schedule_index == schedule_data.count)
                        schedule_index = 0;
                }
            }
        }
    }

    private IEnumerator  MoveCR(AStar.Node target_node)
    {
        if (target_node != null)
        {
            Vector3 target_pos = target_node.world_pos;
            Vector3 dir =target_pos - transform.position;

            animator.SetFloat("dir_x", dir.x);
            animator.SetFloat("dir_y", dir.y);
            //移动距离
            while(Vector3.Distance(target_pos, transform.position)>0.1f)
            {
                transform.position += speed * Time.deltaTime * dir;
                yield return null;
            }
            //每个节点结束处理
            path_nodes.Pop();
            is_moving = false;
            AlignPosToCell();
        }
    }

    public void BuildPath(ScheduleDetail schedule)
    {
        path_nodes.Clear();
        path_nodes = AStar.AStar.instance.BuildPath(current_gridpos, current_schedule.target_gridpos);
    }

    private void OnAfterSceneLoad(object sender, AfterSceneLoadEventArgs e)
    {
        grid=TilemapManager.instance.grid;
    }

    //自动将坐标对齐到网格中心
    private void AlignPosToCell()
    {
        transform.position = (Vector2)current_gridpos + (Vector2)(grid.cellSize / 2);
    }

    private void SetNPCActive()
    {
        sprite_renderer.enabled = true;
        collide.enabled = true;
        //transform.GetChild(0).gameObject.SetActive(true);
    }

    private void SetNPCNotActive()
    {
        sprite_renderer.enabled = false;
        collide.enabled = false;
        //transform.GetChild(0).gameObject.SetActive(false);
    }
}

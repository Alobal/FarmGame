using Item;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Map;
using Crop;

public class Player : MonoBehaviour
{
    #region 运动
    public float speed=5;
    public bool input_enable=true;
    private Vector2 input_move;
    public bool is_moving
    {
        get => input_move != Vector2.zero;
    }
    #endregion

    #region 人物状态
    private Item.SlotUI use_slot;//丢弃物体时需要slot信息，因此这里用的不是ItemDetail
    #endregion

    [SerializeField] private Transform hold_point;
    private Dictionary<int, Action<Vector2>> tool_actions;

    private Rigidbody2D rb;
    [SerializeField]
    private Animator[] animators;//身体各个部位的动画控制器
    private AnimatorOverride animator_override;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animators=GetComponentsInChildren<Animator>();
        animator_override=GetComponent<AnimatorOverride>();
        use_slot = null;
        tool_actions = new()
        {
            [1011] = DigTile,
            [1012] = WaterTile,
        };
    }

    private void Update()
    {
        if (input_enable)
        {
            PlayerInput();
        }
        MoveAnimation();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void OnEnable()
    {
        TransitionManager.BeforeSceneUnload += StopInput;
        TransitionManager.AfterSceneLoad += OnSceneLoad;
        Item.SlotUI.ClickSlot += OnClickSlot;
        CursorManager.ClickMouseLeft += OnMouseLeft;
    }

    private void OnDisable()
    {
        TransitionManager.BeforeSceneUnload -= StopInput;
        TransitionManager.AfterSceneLoad -= OnSceneLoad;
        Item.SlotUI.ClickSlot -= OnClickSlot;
        CursorManager.ClickMouseLeft -= OnMouseLeft;

    }
    /// <summary>
    /// 鼠标点击事件，用于响应交互
    /// </summary>
    /// <param name="args"></param>
    private void OnMouseLeft(ClickMouseLeftEventArgs args)
    {
        Vector3 click_pos = args.world_pos;
        GameObject target_go = args.target_go;
        //执行action
        if (use_slot)//正在使用道具 响应道具功能
        {
            
            input_enable = false;
            if (target_go == null)//点击地面
            {
                switch (use_slot.item_detail.item_type)
                {
                    case ItemType.Seed:
                        SeedTile(click_pos);
                        break;
                    case ItemType.Tool:
                        if (tool_actions.ContainsKey(use_slot.item_detail.id))
                            tool_actions[use_slot.item_detail.id](click_pos);
                        break;
                    case ItemType.BluePrint:
                        MakeBuildingItem(click_pos);
                        break;
                    default://drop action
                        if (target_go == null && use_slot.item_detail.can_drop)
                            ThrowItem(click_pos);
                        break;
                }
            }
            else//点击物体
            {
                if(target_go.GetComponent<CropObject>() is CropObject crop)//点中庄稼
                {
                    HarvsetCrop(crop, click_pos);
                }
            }

            if (use_slot.is_empty)
            {
                use_slot = null;
                animator_override.SwitchAnimator(PlayerAction.Default);
                CursorManager.instance.SetCursor(CursorManager.instance.normal);
            }
            input_enable = true;
        }
    }


    //点击道具栏，修改当前使用道具
    private void OnClickSlot(Item.SlotUI slot)
    {
        if(slot.is_selected && !slot.is_empty)
        {
            use_slot = slot;
        }
        else
        {
            use_slot = null;
        }
    }

    private void PlayerInput()
    {

        input_move=new Vector2(Input.GetAxisRaw("Horizontal"),
                               Input.GetAxisRaw("Vertical")).normalized;

        if (Input.GetKey(KeyCode.LeftShift))
            input_move *= 0.45f;
    }


    private void Move()
    {
        if (!input_enable)
            return;
        //这是一个物理函数，因此假如放在Update中，只会响应FixedUpdate前最后一次call，且deltatime很小，导致很慢
        rb.MovePosition(rb.position + speed * Time.deltaTime * input_move);
    }


    private void StopInput()
    {
        input_enable = false;
    }
    private void OnSceneLoad(object sender, AfterSceneLoadEventArgs e)
    {
        transform.position = e.target_position;
        input_enable = true;
    }


    #region Animator
    private void MoveAnimation()
    {
        foreach (var animator in animators)
        {
            animator.SetBool("is_moving", is_moving);
            if (is_moving)
            {
                animator.SetFloat("input_x", input_move.x);
                animator.SetFloat("input_y", input_move.y);
            }
        }
    }
    private void UseItemAnimation(Vector3 click_pos)
    {
        AllAnimatorSet("using_tool");
        
        FaceDir(click_pos);
    }

    //使角色朝向target_pos
    private void FaceDir(Vector3 target_pos)
    {
        float mouse_dir_x = target_pos.x - transform.position.x;
        float mouse_dir_y = target_pos.y - transform.position.y;
        if (MathF.Abs(mouse_dir_x) > MathF.Abs(mouse_dir_y))
            mouse_dir_y = 0;
        else
            mouse_dir_x = 0;
        AllAnimatorSet("input_x", mouse_dir_x);
        AllAnimatorSet("input_y", mouse_dir_y);
    }

    private void AllAnimatorSet(string param, float value)
    {
        foreach (var animator in animators)
            animator.SetFloat(param, value);
    }

    private void AllAnimatorSet(string param, int value)
    {
        foreach (var animator in animators)
            animator.SetInteger(param, value);
    }

    private void AllAnimatorSet(string param, bool value)
    {
        foreach (var animator in animators)
            animator.SetBool(param, value);
    }
    private void AllAnimatorSet(string param)
    {
        foreach (var animator in animators)
            animator.SetTrigger(param);
    }
    #endregion

    #region Item Actions
    //TODO 怎么管理action
    /// <summary>
    /// 朝目标位置扔出已选物体
    /// </summary>
    /// <param name="target_pos"></param>
    private void ThrowItem(Vector2 target_pos)
    {
        if (Vector2.Distance(target_pos, transform.position) > Settings.throw_radius)
            return;
        ItemObject world_item = WorldItemManager.instance.MakeItem(use_slot.item_detail.id, hold_point.position);
        PackDataManager.instance.PlayerRemoveItemAt(use_slot.index);
        FaceDir(target_pos);
        world_item.Move(target_pos);
    }


    private void MakeBuildingItem(Vector2 target_pos)
    {
        if (Vector2.Distance(target_pos, transform.position) > Settings.throw_radius)
            return;
        WorldItemManager.instance.MakeBuildingItem(use_slot.item_detail.id, target_pos);
        PackDataManager.instance.PlayerRemoveItemAt(use_slot.index);
        FaceDir(target_pos);

    }
    private void DigTile(Vector2 target_pos)
    {
        if (Vector2.Distance(target_pos, transform.position) > use_slot.item_detail.use_radius)
            return;
        //执行对应的角色动画
        TileDetail tile_detail=TilemapManager.instance.GetTileDetail(target_pos);
        if (tile_detail == null)
            return;

        UseItemAnimation(target_pos);
        TilemapManager.instance.SetTileDig(tile_detail);
    }

    private void WaterTile(Vector2 target_pos)
    {
        if (Vector2.Distance(target_pos, transform.position) >use_slot.item_detail.use_radius)
            return;
        TileDetail tile_detail = TilemapManager.instance.GetTileDetail(target_pos);
        if (tile_detail == null || !tile_detail.can_water)
            return;

        UseItemAnimation(target_pos);
        TilemapManager.instance.SetTileWater(tile_detail);
    }
    private void SeedTile(Vector2 target_pos)
    {
        if (Vector2.Distance(target_pos, transform.position) > use_slot.item_detail.use_radius)
            return;
        TileDetail seed_tile = TilemapManager.instance.GetTileDetail(target_pos);
        if (seed_tile == null || !seed_tile.can_seed)
            return;

        FaceDir(target_pos);
        TilemapManager.instance.SetTileSeed(seed_tile, use_slot.item_detail.id);
        //清除物品
        PackDataManager.instance.PlayerRemoveItemAt(use_slot.index);
    }
    private void HarvsetCrop(CropObject crop,Vector2 target_pos)
    {
        if (!crop.CheckHarvestable(use_slot.item_detail.id))
            return ;
        UseItemAnimation(crop.transform.position);
        crop.Harvest(use_slot.item_detail.id);
    }
    #endregion

}

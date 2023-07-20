using Item;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : Singleton<CursorManager>
{
    [SerializeField]
    public Texture2D normal, tool, commodity,seed;
    public Vector2 world_pos;
    public static event Action<ClickMouseLeftEventArgs> ClickMouseLeft; //击中坐标以及击中物体，物体可为null
    // Start is called before the first frame update
    private void OnEnable()
    {
        SlotUI.ClickSlot += OnClickSlot;
    }

    private void Start()
    {
        SetCursor(normal);
        Application.targetFrameRate = 100;
    }
    
    private void OnDisable()
    {
        SlotUI.ClickSlot -= OnClickSlot;
    }

    // Update is called once per frame
    void Update()
    {
        MouseInput();
    }

    private void MouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            world_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);//2D游戏 像素坐标变换到3D坐标
            Ray ray=Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            GameObject target_go=hit? hit.transform.gameObject:null;
            //Debug.Log(target_go);
            ClickMouseLeft.Invoke(new (world_pos,target_go));
        }
    }


    private void OnClickSlot(SlotUI slot)
    {
        Texture2D cursor_texture;
        if (!slot.is_selected || slot.is_empty)
            cursor_texture = normal;
        else
            cursor_texture=slot.item_detail.item_type switch
            {
                ItemType.Seed => seed,
                ItemType.Commodity => commodity,
                ItemType.Furniture => commodity,
                ItemType.Tool => tool,
                _ => normal
            };
        SetCursor(cursor_texture);
    }

 

    public void SetCursor(Texture2D texture)
    {
        Cursor.SetCursor(texture,Vector2.zero,CursorMode.Auto);
    }
}

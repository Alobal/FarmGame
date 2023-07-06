using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 使得挂载的UI能够自由拖拽
/// </summary>
public class DragUI : MonoBehaviour,IDragHandler,IBeginDragHandler,IEndDragHandler
{
    public GameObject drag_target;
    public bool end_reset=false;
    private Vector2 pos_origin;
    private void Awake()
    {
        if (drag_target == null)
            drag_target = gameObject;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        pos_origin = drag_target.transform.position;       
    }
    public void OnDrag(PointerEventData eventData)
    {
        drag_target.transform.position +=  (Vector3)eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (end_reset)
            drag_target.transform.position = pos_origin;
    }
}

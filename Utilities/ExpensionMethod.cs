using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExpensionMethod
{
    public static Vector2 XY(this Vector3 origin)
    {
        return new Vector2 (origin.x, origin.y);
    }

    public static Vector2Int XY(this Vector3Int origin)
    {
        return new (origin.x, origin.y);
    }

    public static string ToText(this ItemType item_type)
    {
        return item_type switch
        {
            ItemType.Tool => "工具",
            _ => item_type.ToString()
        };
    }
}

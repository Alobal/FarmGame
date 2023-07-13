using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExpensionMethod
{

    public static string ToText(this ItemType item_type)
    {
        return item_type switch
        {
            ItemType.Tool => "工具",
            _ => item_type.ToString()
        };
    }


}

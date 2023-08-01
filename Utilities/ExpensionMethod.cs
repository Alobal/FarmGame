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

    //将Sprite转为Texture2D，注意要求Sprite源Texture属性中Advanced/ReadWrite是启用的。
    public static Texture2D ToTexture2D(this Sprite sprite)
    {
        
        var new_texture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
        var pixels = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                    (int)sprite.textureRect.y,
                                                    (int)sprite.textureRect.width,
                                                    (int)sprite.textureRect.height);
        new_texture.SetPixels(pixels);
        new_texture.Apply();
        return new_texture;
    }


}

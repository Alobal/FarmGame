using Map;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class MyUtility
{
    static public (List<Tkey>, List<Tvalue>) DictToList<Tkey, Tvalue>(Dictionary<Tkey, Tvalue> dict)
    {
        List<Tkey> key_list = dict.Keys.ToList();
        List<Tvalue> value_list = dict.Values.ToList();
        return (key_list, value_list);
    }

    static public Dictionary<Tkey, Tvalue> ListToDict<Tkey, Tvalue>(List<Tkey> key_list, List<Tvalue> value_list)
    {
        Dictionary<Tkey, Tvalue> dict = new();
        for (int i = 0; i < key_list.Count; i++)
            dict.Add(key_list[i], value_list[i]);
        return dict;
    }

    //注意 协程
    static public IEnumerator WaitDoCR(Action action,float seconds)
    {
        yield return new WaitForSeconds(seconds);
        action();
    }

    static public bool RandomBool()
    {
        if (UnityEngine.Random.value < 0.5)
            return false;
        else
            return true;
    }

    //自适应调整box碰撞体尺寸
    static public void AdaptiveBoxColliderToSprite(Sprite sprite,BoxCollider2D collide)
    {
        if(sprite==null || collide == null)
            return;
        Vector2 coll_size =sprite.bounds.size;
        collide.size = coll_size;
        collide.offset = new Vector2(0, sprite.bounds.center.y);
    }


}
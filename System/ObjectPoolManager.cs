using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    public List<GameObject> prefabs;
    private  Dictionary<GameObject,ObjectPool<GameObject>> pools=new ();

    private new void Awake()
    {
        base.Awake();
        InitPools();
    }

    private void InitPools()
    {
        foreach (GameObject go in prefabs)
        {
            Transform parent = new GameObject(go.name).transform;
            parent.SetParent(transform);
            ObjectPool<GameObject> pool = new(
                () => Instantiate(go,parent),
                e => e.gameObject.SetActive(true),
                e => e.gameObject.SetActive(false),
                e => Destroy(e)
                );
            pools.Add(go,pool);
        }
    }


    public GameObject Get(GameObject prefab,Vector2 pos,float delay_seconds=0.0f)
    {
        GameObject new_object;
        try
        {
            new_object = pools[prefab].Get();
        }
        catch (Exception)
        {
            Debug.Log("对象池中没有该类型");
            throw new NullReferenceException("对象池中没有该类型");
        }
        new_object.transform.position= pos;
        if( delay_seconds > 0 )
            StartCoroutine(ReleaseCR(prefab,new_object,delay_seconds));
        return new_object;
    }

    public IEnumerator ReleaseCR(GameObject prefab,GameObject instance,float delay_seconds)
    {
        yield return new WaitForSeconds(delay_seconds);
        pools[prefab].Release(instance);
    }

}

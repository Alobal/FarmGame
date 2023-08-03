using Audio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;


/// <summary>
/// 为所有存储的Prefab进行对象池化管理。
/// </summary>
public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    public List<GameObject> prefabs;
    private  Dictionary<GameObject,ObjectPool<GameObject>> pools=new ();
    [SerializeField] GameObject sound_prefab;

    private new void Awake()
    {
        base.Awake();
        InitPools();
    }

    //根据序列化的prefabs初始化不同的ObjectPool
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

    /// <summary>
    /// 通过对象池获取一个实例对象。
    /// </summary>
    /// <param name="prefab">实例的prefab</param>
    /// <param name="pos">实例的生成位置</param>
    /// <param name="release_seconds">销毁时间</param>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    public GameObject Get(GameObject prefab,Vector2 pos,float release_seconds=0.0f)
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
        if( release_seconds > 0 )
            StartCoroutine(ReleaseCR(prefab,new_object,release_seconds));
        return new_object;
    }

    /// <summary>
    /// 生成音效
    /// </summary>
    /// <param name="sound_name"></param>
    public void GetSound(SoundName sound_name)
    {
        var sound_config = AudioManager.instance.audio_configs.Get(sound_name);
        GameObject sound_object=pools[sound_prefab].Get();
        Sound sound= sound_object.GetComponent<Sound>();
        sound.Init(sound_config);
        sound.Play();
        StartCoroutine(ReleaseCR(sound_prefab, sound_object, sound_config.sound_clip.length));
    }

    public IEnumerator ReleaseCR(GameObject prefab,GameObject instance,float delay_seconds)
    {
        yield return new WaitForSeconds(delay_seconds);
        pools[prefab].Release(instance);
    }

}

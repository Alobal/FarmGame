using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T instance
    {
        get;
        private set;
    }
    protected virtual void Awake()
    {
        if (instance == null)
            instance = (T)this;
        else
            Destroy(gameObject);
    }

    protected virtual  void OnDestroy()
    {
        if(instance == this)
            instance = null;
    }
}

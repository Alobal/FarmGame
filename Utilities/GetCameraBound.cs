using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetCameraBound : MonoBehaviour
{

    private void OnEnable()
    {
        TransitionManager.AfterSceneLoad += GetConfinerBound;
    }
    private void OnDisable()
    {
        TransitionManager.AfterSceneLoad -= GetConfinerBound;
    }
    private void GetConfinerBound(object sender,AfterSceneLoadEventArgs e)
    { 
        if (GameObject.FindGameObjectWithTag("CameraBound") is GameObject bound_object)
        {
            PolygonCollider2D bound = bound_object.GetComponent<PolygonCollider2D>();
            CinemachineConfiner confiner= GetComponent<CinemachineConfiner>();
            confiner.m_BoundingShape2D = bound;
            confiner.InvalidatePathCache();//clean the cache.
        }
    }
}

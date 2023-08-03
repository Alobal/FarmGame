using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class TransitionPoint : MonoBehaviour
{
    [SceneName]
    public string origin_scene;
    [SceneName]
    public string target_scene;
    public Vector3 target_position;

    private void Start()
    {
        origin_scene=gameObject.scene.name;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            TransitionManager.instance.StartTransition(origin_scene,target_scene);
            collision.transform.position = target_position;
        }
    }
}

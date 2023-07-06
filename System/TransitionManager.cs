using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : Singleton<TransitionManager>
{
    public static event Action BeforeSceneUnload;
    public static event EventHandler<AfterSceneLoadEventArgs> AfterSceneLoad;

    private void Start()
    {
        //StartCoroutine(LoadSceneSetActive("Field1"));
    }

    public  void  StartTransition(string origin_scene, string target_scene, Vector3 target_position)
    {
        StartCoroutine(Transition(origin_scene, target_scene, target_position));    
    }

    private IEnumerator Transition(string origin_scene,string target_scene,Vector3 target_position)
    {
        BeforeSceneUnload.Invoke();
        yield return SceneManager.UnloadSceneAsync(origin_scene);

        yield return LoadSceneSetActive(target_scene);
        AfterSceneLoad.Invoke(this,new (origin_scene,target_scene,target_position));
    }
    private IEnumerator LoadSceneSetActive(string scene_name)
    {
        if(SceneManager.GetSceneByName(scene_name).isLoaded == false)
            yield return SceneManager.LoadSceneAsync(scene_name, LoadSceneMode.Additive);
        Scene new_scene = SceneManager.GetSceneByName(scene_name);
        SceneManager.SetActiveScene(new_scene);
    }
}

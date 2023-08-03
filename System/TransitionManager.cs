using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : Singleton<TransitionManager>
{
    public static event Action BeforeSceneUnload;
    public static event EventHandler<AfterSceneLoadEventArgs> AfterSceneLoad;//NOTE Awake在场景加载后最先调用，比起AfterSceneLoad还早。

    private new void Awake()
    {
        base.Awake();

    }

    private void Start()
    {
        //NOTE 注意放在Awake中则检测不到预载的UIScene
        if (SceneManager.GetSceneByName("UIScene").isLoaded == false)
            SceneManager.LoadScene("UIScene", LoadSceneMode.Additive);
        //StartCoroutine(LoadSceneSetActive("Field1"));
    }

    public  void  StartTransition(string origin_scene, string target_scene)
    {
        StartCoroutine(Transition(origin_scene, target_scene));    
    }

    private IEnumerator Transition(string origin_scene,string target_scene)
    {
        BeforeSceneUnload.Invoke();
        if(origin_scene != null && origin_scene != string.Empty)
            yield return SceneManager.UnloadSceneAsync(origin_scene);

        yield return LoadSceneSetActive(target_scene);
        AfterSceneLoad.Invoke(this,new (origin_scene,target_scene));
    }
    public IEnumerator LoadSceneSetActive(string scene_name)
    {
        if(SceneManager.GetSceneByName(scene_name).isLoaded == false)
            yield return SceneManager.LoadSceneAsync(scene_name, LoadSceneMode.Additive);
        Scene new_scene = SceneManager.GetSceneByName(scene_name);
        SceneManager.SetActiveScene(new_scene);
    }
}

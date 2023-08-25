using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Audio;

public class AssetBundleManager : MonoBehaviour
{

    private static string root_path = Application.streamingAssetsPath;

    private void Start()
    {
        //LoadAsset("sound");
    }

#if UNITY_EDITOR
    [MenuItem("AssetBundle/Build AB")]
    static void BuildAB()
    {
        if (!Directory.Exists(root_path))
        {
            Directory.CreateDirectory(root_path);
        }

        AssetDatabase.Refresh();

        BuildPipeline.BuildAssetBundles(root_path, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
    }
#endif

    public  void LoadAsset(string name)
    {
        string path = $"{root_path}/{name}";
        AssetBundle ab = AssetBundle.LoadFromFile(path);
        Sound[] sounds=ab.LoadAllAssets<Sound>();
        Debug.Log(ab.LoadAsset("Axe"));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SceneNameAttribute))]
public class SceneNameDrawer : PropertyDrawer
{
    int scene_index = -1;
    GUIContent[] scene_names;
    readonly string[] scene_path_splite = { "/", ".unity" };
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {   
        //FIX List中同样字段会错误同步更改
        if (EditorBuildSettings.scenes.Length == 0) return;
        if(scene_index == -1)//没有预设值则进行初始化
        {
            Init(property);
        }
        //new_index为用户点击选择时返回的index
        int new_index = EditorGUI.Popup(position, label, scene_index, scene_names);
        if (new_index!=scene_index)
        {
            scene_index = new_index;
            property.stringValue = scene_names[scene_index].text;
        }
    }

    /// <summary>
    /// 初始化scene_names，并且根据已有的预设值情况进行初始化
    /// </summary>
    /// <param name="property"></param>
    private void Init(SerializedProperty property)
    {   
        //获取所有scene_names
        var scenes=EditorBuildSettings.scenes;
        if (scenes.Length == 0)
        {
            scene_names = new[] { new GUIContent("Build Settings Scene is None") };
            return;
        }
        scene_names = new GUIContent[scenes.Length];
        for(int i=0;i<scene_names.Length;i++) 
        {
            string[] splits = scenes[i].path.Split(scene_path_splite,System.StringSplitOptions.RemoveEmptyEntries);
            string name =  splits.Length>0 ? splits[^1] : "Deleted File";
            scene_names[i]= new GUIContent(name);
        }

        //如果没有预设值 则设为0号scene
        if (string.IsNullOrEmpty(property.stringValue))
        {
            scene_index = 0;
        }
        else //已有预设值 自动选择对应scene
        { 
            for(int i=0;i<scene_names.Length;i++)
            {
                if (property.stringValue == scene_names[i].text)
                {
                    scene_index = i;
                    break;
                }
            }
            if(scene_index==-1)
                scene_index = 0;
        }
    }
}
#endif
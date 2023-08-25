using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;
using System;

[CustomEditor(typeof(MonoBehaviour),true)]
[CanEditMultipleObjects]
public class InspectorButton : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var mono = target as MonoBehaviour;
        if (mono == null)
            return;
        //获取定义了InspectorButtonAttribute特性的方法
        var methods =mono.GetType().GetMethods(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static
            ).Where(method => Attribute.IsDefined(method,typeof(InspectorButtonAttribute))).ToArray();

        foreach ( var method in methods )
        {
            //对有特性的方法绘制button
            var attr=method.GetCustomAttribute<InspectorButtonAttribute>();
            DrawButton(method, attr.Name);
        }
    }

    private void DrawButton(MethodInfo method_info,string button_name)
    {
        if (string.IsNullOrEmpty(button_name))
            button_name = method_info.Name;

        EditorGUILayout.BeginHorizontal();

        if(GUILayout.Button(button_name,GUILayout.ExpandWidth(true)))
        {
            foreach (var target in targets)
            {
                if (target is MonoBehaviour mono)
                { 
                    var val =method_info.Invoke(mono, null);
                    if (val is IEnumerator coroutine)
                        mono.StartCoroutine(coroutine);
                    else if (val != null)
                        Debug.Log($"{button_name} 调用结果: {val}");
                }

            }
        }

        EditorGUILayout.EndHorizontal();
    }
}

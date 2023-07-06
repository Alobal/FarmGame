using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.AttributeUsage(System.AttributeTargets.Method)]
public class InspectorButtonAttribute : PropertyAttribute
{
    public readonly string Name;
    public InspectorButtonAttribute(string name)
    {
        Name = name;
    }
}

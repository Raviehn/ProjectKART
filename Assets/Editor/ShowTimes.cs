using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ValueOptimizer))]
public class ShowTimes : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ValueOptimizer vo = (ValueOptimizer)target;
        if (GUILayout.Button("Show Times"))
        {
            vo.ShowTimes();
        }
    }
}
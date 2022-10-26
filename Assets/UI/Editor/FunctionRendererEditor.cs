using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FunctionRenderer))]
public class FunctionRendererEditor : Editor
{
    public override void OnInspectorGUI()
    {
        FunctionRenderer fr = (FunctionRenderer)target;

        if (DrawDefaultInspector())
        {
            fr.Start();
            fr.tagRenderer.Start();
        }
    }
}

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FunctionRenderer))]
public class FunctionRendererEditor : UnityEditor.Editor
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

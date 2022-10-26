using UnityEditor;

[CustomEditor(typeof(FunctionTagsRenderer))]
public class FunctionTagsRendererEditor : Editor
{
    public override void OnInspectorGUI()
    {
        FunctionTagsRenderer fr = (FunctionTagsRenderer)target;

        if (DrawDefaultInspector())
        {
            fr.Start();
        }
    }
}
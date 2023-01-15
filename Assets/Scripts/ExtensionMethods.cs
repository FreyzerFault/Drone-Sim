using UnityEngine;

public static class ExtensionMethods
{
    // Normalize angle [-180,180]
    public static float normalizeAngle(this float angle)
    {
        if (angle > 180)
            angle -= 360;
        else if (angle < -180)
            angle += 360;

        return angle;
    }

    public static Vector3 normalizeAngles(this Vector3 eulerRotation)
    {
        return new Vector3(
            eulerRotation.x.normalizeAngle(),
            eulerRotation.y.normalizeAngle(),
            eulerRotation.z.normalizeAngle()
        );
    }
    
    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
    
    public static void ClearChildren(this Transform transform) 
    {
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
            Object.DestroyImmediate(transform.GetChild(0).gameObject);
    }
}

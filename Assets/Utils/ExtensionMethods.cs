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
    
    // Convierte una coordenada dentro de un circulo extendiendola a un cuadrado
    public static Vector2 circleToSquareVector(this Vector2 input)
    {
        // Transformation from in-circle vector to in-square vector:
        // One of the components (x or y), the maximum of the two, is set to |v|
        // The other can be calculated by Pitagoras' theorem as:
        // x = y / tan(x) | y = tan(x) * x
        float angle = Mathf.Atan2(input.y, input.x);
        float angleDeg = angle * Mathf.Rad2Deg;
        float magnitude = input.magnitude;
            
        float x, y;
        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
        {
            x = angleDeg is < 90 and > -90 ? magnitude : -magnitude;
            y = Mathf.Tan(angle) * x;
        }
        else if (Mathf.Abs(input.y) > Mathf.Abs(input.x))
        {
            y = angle > 0 ? magnitude : -magnitude;
            x = y / Mathf.Tan(angle);
        }
        else
        {
            x = angleDeg is < 90 and > -90 ? magnitude : -magnitude;
            y = angle > 0 ? magnitude : -magnitude;
        }

        return new Vector2(x, y);
    }
    
    public static Vector2Int ParseResolution(this string resolutionText)
    {
        string[] res = resolutionText.Split("x");
        int width = int.Parse(res[0]);
        int height = int.Parse(res[1]);
        return new Vector2Int(width, height);
    }
}

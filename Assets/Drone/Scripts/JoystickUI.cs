using UnityEngine;

public class JoystickUI : MonoBehaviour
{
    private RectTransform square;
    private RectTransform pointObj;

    private float radius;

    private void Awake()
    {
        square = GetComponent<RectTransform>();
        pointObj = transform.GetChild(0).GetComponent<RectTransform>();
        
        radius = square.rect.width / 2;
    }
    
    
    public void SetJoystick(Vector2 coord) => pointObj.transform.localPosition = coord * radius;

    // Simula que los input se hacen en un cuadrado como un mando de un dron, y no en un circulo como el mando de consola 
    public void SetJoystickSquared(Vector2 coord) => SetJoystick(circleToSquareInput(coord));

    // Convierte una coordenada dentro de un circulo extendiendola a un cuadrado
    private static Vector2 circleToSquareInput(Vector2 input)
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
}

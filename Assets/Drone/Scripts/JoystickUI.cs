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
    public void SetJoystickSquared(Vector2 coord) => SetJoystick(coord.circleToSquareVector());
    // public void SetJoystickSquared(Vector2 coord) => SetJoystick(coord);
}

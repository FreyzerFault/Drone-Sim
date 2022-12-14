using UnityEngine;
using UnityEngine.InputSystem;

public class DroneInputControllerTest : MonoBehaviour
{
    public DroneControllerTest droneControllerTest;
    
    public RectTransform joystickL;
    public RectTransform joystickR;

    private float joystickSquareWidth;
    
    private void Awake()
    {
        droneControllerTest = GetComponent<DroneControllerTest>();
        joystickSquareWidth = joystickL.transform.parent.GetComponent<RectTransform>().rect.width / 2;
    }

    #region InputMessages

    private void OnMove(InputValue value)
    {
        Vector2 move = circleToSquareInput(value.Get<Vector2>());
        droneControllerTest.pitchPower = move.y;
        droneControllerTest.rollPower = move.x;
        
        joystickR.transform.localPosition = move * joystickSquareWidth;
    }

    private void OnLiftYaw(InputValue value)
    {
        Vector2 liftYaw = circleToSquareInput(value.Get<Vector2>());
        droneControllerTest.yawPower = liftYaw.x;
        droneControllerTest.liftPower = liftYaw.y;
        
        joystickL.transform.localPosition = liftYaw * joystickSquareWidth;
    }

    private void OnReset(InputValue value)
    {
        droneControllerTest.Reset();
    }

    private void OnToggleMotor(InputValue value)
    {
        droneControllerTest.enabled = !droneControllerTest.enabled;
    }

    private void OnChangeMode(InputValue value)
    {
        droneControllerTest.stabilization = !droneControllerTest.stabilization;
    }

    #endregion

    #region Utils

    private Vector2 circleToSquareInput(Vector2 input)
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

    #endregion
}

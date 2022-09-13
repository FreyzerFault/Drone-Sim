using UnityEngine;
using UnityEngine.InputSystem;

public class DroneInputController : MonoBehaviour
{
    
    private DroneController droneController;

    private void Awake()
    {
        droneController = GetComponent<DroneController>();
    }

    private void OnMove(InputValue value)
    {
        Vector2 move = value.Get<Vector2>();
        droneController.rollInput = move.x;
        droneController.pitchInput = move.y;
    }

    private void OnRotate(InputValue value)
    {
        droneController.yawInput = value.Get<float>();
    }
    
    private void OnLift(InputValue value)
    {
        droneController.liftInput = value.Get<float>();
    }

    private void OnReset(InputValue value)
    {
        droneController.ResetRotation();
    }

    private void OnToggleMotor(InputValue value)
    {
        droneController.enabled = !droneController.enabled;
    }
}

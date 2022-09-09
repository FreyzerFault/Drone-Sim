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
        droneController.roll = move.x;
        droneController.pitch = move.y;
    }

    private void OnRotate(InputValue value)
    {
        droneController.yaw = value.Get<float>();
    }
    
    private void OnLift(InputValue value)
    {
        droneController.lift = value.Get<float>();
    }
}

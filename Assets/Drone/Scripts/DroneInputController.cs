using UnityEngine;
using UnityEngine.InputSystem;

namespace DroneSim
{
    [RequireComponent(typeof(PlayerInput))]
    public class DroneInputController : MonoBehaviour
    {
        private DroneController droneController;

        [HideInInspector] public bool cheatsActivated = false;

        private void Start() => droneController = GetComponent<DroneController>();

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnPause -= Disable;
                GameManager.Instance.OnUnpause -= Enable;
            }
        }

        private void Enable() => GetComponent<PlayerInput>().enabled = true;
        private void Disable() => GetComponent<PlayerInput>().enabled = false;

        #region InputMessages

        private void OnPitchRoll(InputValue value)
        {
            if (GameManager.GameIsPaused) return; 
            Vector2 joystick = value.Get<Vector2>();
            droneController.rollInput = joystick.x;
            droneController.pitchInput = joystick.y;
        }

        private void OnLiftYaw(InputValue value)
        {
            if (GameManager.GameIsPaused) return; 
            Vector2 joystick = value.Get<Vector2>();
            droneController.yawInput = joystick.x;
            droneController.liftInput = joystick.y;
        }

        private void OnReset()
        {
            if (GameManager.GameIsPaused) return; 
            droneController.ResetRotation();
        }

        private void OnToggleMotor()
        {   
            if (GameManager.GameIsPaused) return; 
            droneController.enabled = !droneController.enabled;
        }

        private void OnChangeMode(InputValue value)
        {
            if (GameManager.GameIsPaused) return; 
            if (cheatsActivated)
                droneController.ActivateGodMode();
            else
                droneController.SwitchMode(value.Get<float>() > 0);
        }
        
        private void OnToggleHover()
        {   
            if (GameManager.GameIsPaused) return; 
            droneController.ToggleHoverStabilization();
        }

        private void OnSwitchCamera()
        {
            if (GameManager.GameIsPaused) return; 
            CameraManager.Instance.SwitchCamera();
        }
        
        
        // PAUSE
        public void OnPause() => PauseMenu.Instance.Toggle();

        public void OnHoldToCheats(InputValue value) => cheatsActivated = !cheatsActivated;

        #endregion
    }
}

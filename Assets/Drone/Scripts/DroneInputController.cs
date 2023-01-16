using UnityEngine;
using UnityEngine.InputSystem;

namespace DroneSim
{
    public class DroneInputController : MonoBehaviour
    {
        private DroneController droneController;

        [HideInInspector] public bool cheatsActivated = false;
        
        private void Awake()
        {
            droneController = GetComponent<DroneController>();
        }

        #region InputMessages

        private void OnPitchRoll(InputValue value)
        {
            Vector2 joystick = value.Get<Vector2>();
            droneController.rollInput = joystick.x;
            droneController.pitchInput = joystick.y;
        }

        private void OnLiftYaw(InputValue value)
        {
            Vector2 joystick = value.Get<Vector2>();
            droneController.yawInput = joystick.x;
            droneController.liftInput = joystick.y;
        }

        private void OnReset()
        {
            if (GameManager.Instance.GameIsPaused) return;
            
            droneController.ResetRotation();
        }

        private void OnToggleMotor()
        {
            if (GameManager.Instance.GameIsPaused) return;
            
            droneController.enabled = !droneController.enabled;
        }

        private void OnChangeMode(InputValue value)
        {
            if (GameManager.Instance.GameIsPaused) return;

            if (cheatsActivated)
                droneController.ActivateGodMode();
            else
                droneController.SwitchMode(value.Get<float>() > 0);
        }
        
        private void OnToggleHover()
        {
            if (GameManager.Instance.GameIsPaused) return;
            
            droneController.ToggleHoverStabilization();
        }

        private void OnSwitchCamera()
        {
            if (GameManager.Instance.GameIsPaused) return;
            
            droneController.cameraManager.SwitchCamera();
        }
        
        
        // PAUSE
        private MenuToggle menuToggle;
        public void OnPause(InputValue value)
        {
            if (menuToggle == null)
                menuToggle = GameObject.FindGameObjectWithTag("MenuToggle").GetComponent<MenuToggle>();

            menuToggle.Toggle();
        }

        public void OnHoldToCheats(InputValue value) => cheatsActivated = !cheatsActivated;

        #endregion
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

namespace DroneSim
{
    public class DroneInputController : MonoBehaviour
    {
        private DroneController droneController;

        private JoystickUI joystickUILeft;
        private JoystickUI joystickUIRight;
        
        private void Awake()
        {
            droneController = GetComponent<DroneController>();

            GameObject[] joysticks = GameObject.FindGameObjectsWithTag("Joystick");
            if (joysticks.Length != 2) Debug.LogError("Joysticks are not found in UI");
            joystickUILeft = joysticks[1].GetComponent<JoystickUI>();
            joystickUIRight = joysticks[0].GetComponent<JoystickUI>();
        }

        #region InputMessages

        private void OnPitchRoll(InputValue value)
        {
            Vector2 joystick = value.Get<Vector2>();
            droneController.rollInput = joystick.x;
            droneController.pitchInput = joystick.y;
            
            joystickUIRight.SetJoystickSquared(joystick);
        }

        private void OnLiftYaw(InputValue value)
        {
            Vector2 joystick = value.Get<Vector2>();
            droneController.yawInput = joystick.x;
            droneController.liftInput = joystick.y;
            
            joystickUILeft.SetJoystickSquared(joystick);
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
            
            if (droneController.FPVCamera.gameObject.activeSelf)
                GameManager.Camera = droneController.TPVCamera;
            else if (droneController.TPVCamera.gameObject.activeSelf)
                GameManager.Camera = droneController.StaticCamera;
            else if (droneController.StaticCamera.gameObject.activeSelf)
                GameManager.Camera = droneController.FPVCamera;
        }
        
        
        // PAUSE
        private MenuToggle menuToggle;
        public void OnPause(InputValue value)
        {
            if (menuToggle == null)
                menuToggle = GameObject.FindGameObjectWithTag("MenuToggle").GetComponent<MenuToggle>();

            menuToggle.Toggle();
        }

        #endregion
    }
}

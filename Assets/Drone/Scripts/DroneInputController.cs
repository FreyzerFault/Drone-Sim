using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DroneSim
{
    public class DroneInputController : MonoBehaviour
    {
        private DroneController droneController;

        [HideInInspector] public bool cheatsActivated = false;

        private void Start()
        {
            droneController = GetComponent<DroneController>();

            GameManager.Instance.OnPause += disable;
            GameManager.Instance.OnUnpause += enable;
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnPause -= disable;
                GameManager.Instance.OnUnpause -= enable;
            }
        }

        private void enable() => GetComponent<PlayerInput>().enabled = true;
        private void disable() => GetComponent<PlayerInput>().enabled = false;

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
            droneController.ResetRotation();
        }

        private void OnToggleMotor()
        {   
            droneController.enabled = !droneController.enabled;
        }

        private void OnChangeMode(InputValue value)
        {
            if (cheatsActivated)
                droneController.ActivateGodMode();
            else
                droneController.SwitchMode(value.Get<float>() > 0);
        }
        
        private void OnToggleHover()
        {   
            droneController.ToggleHoverStabilization();
        }

        private void OnSwitchCamera()
        {
            droneController.cameraManager.SwitchCamera();
        }
        
        
        // PAUSE
        public void OnPause()
        {
            PauseMenu.Instance.Toggle();
        }

        public void OnHoldToCheats(InputValue value) => cheatsActivated = !cheatsActivated;

        #endregion
    }
}

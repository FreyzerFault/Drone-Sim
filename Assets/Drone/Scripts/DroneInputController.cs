using UnityEngine;
using UnityEngine.InputSystem;

namespace DroneSim
{
    public class DroneInputController : MonoBehaviour
    {
        private DroneController droneController;
        
        public RectTransform joystickL;
        public RectTransform joystickR;

        private float joystickSquareWidth;
        
        private void Awake()
        {
            droneController = GetComponent<DroneController>();
            joystickSquareWidth = joystickL.transform.parent.GetComponent<RectTransform>().rect.width / 2;
        }

        #region InputMessages

        private void OnMove(InputValue value)
        {
            //Vector2 move = value.Get<Vector2>();
            Vector2 move = circleToSquareInput(value.Get<Vector2>());
            droneController.rollInput = move.x;
            droneController.pitchInput = move.y;
            
            joystickR.transform.localPosition = move * joystickSquareWidth;
        }

        private void OnLiftYaw(InputValue value)
        {
            //Vector2 liftYaw = value.Get<Vector2>();
            Vector2 liftYaw = circleToSquareInput(value.Get<Vector2>());
            droneController.yawInput = liftYaw.x;
            droneController.liftInput = liftYaw.y;
            
            joystickL.transform.localPosition = liftYaw * joystickSquareWidth;
        }

        private void OnReset(InputValue value)
        {
            if (GameManager.Instance.IsPaused) return;
            
            droneController.ResetRotation();
        }

        private void OnToggleMotor(InputValue value)
        {
            if (GameManager.Instance.IsPaused) return;
            
            droneController.enabled = !droneController.enabled;
        }

        private void OnChangeMode(InputValue value)
        {
            if (GameManager.Instance.IsPaused) return;

            droneController.SwitchMode(value.Get<float>() > 0);
        }
        
        private void OnToggleHover(InputValue value)
        {
            if (GameManager.Instance.IsPaused) return;
            
            droneController.hoverStabilization = !droneController.hoverStabilization;
        }

        private void OnSwitchCamera(InputValue value)
        {
            if (GameManager.Instance.IsPaused) return;
            
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
}

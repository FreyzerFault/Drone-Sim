using UnityEngine;

namespace DroneSim
{
    public class DroneStaticCamera : DroneCamera
    {
        protected override void Awake()
        {
            base.Awake();
            type = CameraType.Static;
        }

        public override void Enable()
        {
            base.Enable();

            virtualCamera.LookAt = FindObjectOfType<DroneController>().transform;
        }
    }

}
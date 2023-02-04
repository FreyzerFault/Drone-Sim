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
            
            GameObject initObj = GameObject.FindWithTag("Land Camera Init Point");
            if (initObj)
                transform.SetPositionAndRotation(initObj.transform.position, initObj.transform.rotation);

            virtualCamera.LookAt = FindObjectOfType<DroneController>().transform;
        }
    }

}
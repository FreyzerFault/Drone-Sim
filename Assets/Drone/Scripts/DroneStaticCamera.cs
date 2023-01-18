using UnityEngine;

namespace DroneSim
{
    public class DroneStaticCamera : DroneCamera
    {
        protected override void OnEnable() {}

        protected override void LateUpdate()
        {
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(dron.transform.position - transform.position), 0.1f);
        }
    }

}
using UnityEngine;

namespace DroneSim
{
    public class DroneStaticCamera : DroneCamera
    {
        protected override void OnEnable()
        {
            GameObject initObj = GameObject.FindWithTag("Land Camera Init Point");
            if (initObj)
                transform.SetPositionAndRotation(initObj.transform.position, initObj.transform.rotation);
        }

        protected override void LateUpdate()
        {
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(Dron.transform.position - transform.position), 0.1f);
        }
    }

}
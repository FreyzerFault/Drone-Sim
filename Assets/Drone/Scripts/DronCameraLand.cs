using System;
using UnityEngine;

namespace DroneSim
{
    public class DronCameraLand : MonoBehaviour
    {
        private DroneController drone;

        private void Awake()
        {
            if (drone == null) drone = transform.parent.GetChild(0).GetComponent<DroneController>();
        }

        private void LateUpdate()
        {
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(drone.transform.position - transform.position), 0.1f);
        }
    }

}
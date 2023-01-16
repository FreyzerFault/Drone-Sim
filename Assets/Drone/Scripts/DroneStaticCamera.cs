using UnityEngine;

namespace DroneSim
{
    public class DroneStaticCamera : MonoBehaviour
    {
        private DroneController drone;

        private void Awake()
        {
            drone = GameObject.FindGameObjectWithTag("Drone").GetComponent<DroneController>();
        }

        private void LateUpdate()
        {
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(drone.transform.position - transform.position), 0.1f);
        }
    }

}
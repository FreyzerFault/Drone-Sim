using UnityEngine;


namespace DroneSim
{
    public class DroneFpvCamera : MonoBehaviour
    {
        public DroneController drone;

        private void Awake()
        {
            drone = GameObject.FindGameObjectWithTag("Drone").GetComponent<DroneController>();
        }

        private void LateUpdate()
        {
            transform.position = drone.FPVposition.position;
            transform.rotation = drone.FPVposition.rotation;
        }
    }
}

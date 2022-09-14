using UnityEngine;


namespace DroneSim
{
    public class DronCamera1stP : MonoBehaviour
    {
        public DroneController drone;

        public Vector3 positionOffset;
        public float angle;

        private void Awake()
        {
            if (drone == null)
                drone = transform.parent.GetChild(0).GetComponent<DroneController>();

            positionOffset = transform.localPosition;
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }

        Vector3 smoothVel = Vector3.zero;

        private void LateUpdate()
        {
            transform.position = drone.transform.position + drone.transform.forward * positionOffset.x +
                                 drone.transform.up * positionOffset.y;
            transform.rotation = drone.transform.rotation * Quaternion.Euler(angle, 0, 0);
        }
    }
}

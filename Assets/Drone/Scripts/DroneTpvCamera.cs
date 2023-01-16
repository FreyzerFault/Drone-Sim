using UnityEngine;

namespace DroneSim
{
    public class DroneTpvCamera : MonoBehaviour
    {
        public DroneController drone;

        public float distanceOrbit;
        public float heightOrbit;

        [Range(0.001f, 0.1f)] public float smoothness = 1;
        
        // Desplazamiento ligero de la camara en direccion a la que se mueve el dron para facilitar mayor visibilidad
        [Range(0.001f, 0.1f)] public float velocityOffset = 1;
        
        private void Awake()
        {
            drone = GameObject.FindGameObjectWithTag("Drone").GetComponent<DroneController>();

            distanceOrbit = Vector3.ProjectOnPlane(drone.transform.position - drone.TPVposition.position, Vector3.up).magnitude;
            heightOrbit = Vector3.ProjectOnPlane(drone.transform.position - drone.TPVposition.position, drone.transform.forward).magnitude;


            transform.position = drone.transform.position
                                 - Vector3.ProjectOnPlane(drone.transform.forward, Vector3.up).normalized * distanceOrbit
                                 + Vector3.up * heightOrbit;
            transform.rotation = drone.transform.rotation;

            targetPos = drone.transform.position;
        }

        Vector3 smoothVel = Vector3.zero;
        Vector3 smoothTargetVel = Vector3.zero;
        private Vector3 targetPos;

        private void LateUpdate()
        {
            Vector3 droneVelocity = drone.accelerometer.Velocity;
            Vector3 localDroneVelocity = drone.transform.worldToLocalMatrix * droneVelocity;
            localDroneVelocity.z = 0;
            droneVelocity = drone.transform.localToWorldMatrix * localDroneVelocity;
            
            Vector3 dronePos = drone.transform.position;
            Vector3 droneForward = drone.transform.forward;
            
            // Adonde apunta
            targetPos = Vector3.SmoothDamp(targetPos, (dronePos + droneVelocity * velocityOffset),
                ref smoothTargetVel, smoothness);

            // Adonde se mueve
            Vector3 movingTargetPos = dronePos
                                - Vector3.ProjectOnPlane(droneForward, Vector3.up).normalized * distanceOrbit
                                + Vector3.up * heightOrbit;
            
            movingTargetPos += droneVelocity * velocityOffset;

            transform.position =
                Vector3.SmoothDamp(
                    transform.position,
                    movingTargetPos,
                    ref smoothVel, smoothness
                );


            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(targetPos - transform.position), Time.deltaTime * 10);
        }

        #region Gizmos

        private void OnDrawGizmos()
        {
            // Gizmos.color = Color.green;
            // Gizmos.DrawLine(drone.transform.position, targetPos);
            //
            // Gizmos.color = Color.red;
            // Gizmos.DrawSphere(targetPos, 0.02f);
            //
            // Gizmos.color = Color.cyan;
            // Gizmos.DrawSphere(drone.transform.position + drone.accelerometer.Velocity * velocityOffset, 0.02f);
        }

        #endregion
    }
}

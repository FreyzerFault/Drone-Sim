using DroneSim;
using UnityEngine;

namespace dronSim
{
    public class DroneTpvCamera : DroneCamera
    {
        public float distanceOrbit;
        public float heightOrbit;

        [Range(0.001f, 0.1f)] public float smoothness = 1;
        
        // Desplazamiento ligero de la camara en direccion a la que se mueve el dron para facilitar mayor visibilidad
        [Range(0.001f, 0.1f)] public float velocityOffset = 1;
        

        protected void Start()
        {
            DroneController dron = Dron;
            distanceOrbit = Vector3.ProjectOnPlane(dron.transform.position - dron.TPVposition.position, Vector3.up).magnitude;
            heightOrbit = Vector3.ProjectOnPlane(dron.transform.position - dron.TPVposition.position, dron.transform.forward).magnitude;

            targetPos = dron.transform.position;
        }

        Vector3 smoothVel = Vector3.zero;
        Vector3 smoothTargetVel = Vector3.zero;
        private Vector3 targetPos;

        protected override void OnEnable()
        {
            DroneController dron = Dron;
            if (dron != null)
            {
                transform.position = dron.TPVposition.position;
                transform.rotation = dron.TPVposition.rotation;
            }
        }

        protected override void LateUpdate()
        {
            DroneController dron = Dron;
            Vector3 dronVelocity = dron.accelerometer.Velocity;
            Vector3 localdronVelocity = dron.transform.worldToLocalMatrix * dronVelocity;
            localdronVelocity.z = 0;
            dronVelocity = dron.transform.localToWorldMatrix * localdronVelocity;
            
            Vector3 dronPos = dron.transform.position;
            Vector3 dronForward = dron.transform.forward;
            
            // Adonde apunta
            targetPos = Vector3.SmoothDamp(targetPos, (dronPos + dronVelocity * velocityOffset),
                ref smoothTargetVel, smoothness);

            // Adonde se mueve
            Vector3 movingTargetPos = dronPos
                                - Vector3.ProjectOnPlane(dronForward, Vector3.up).normalized * distanceOrbit
                                + Vector3.up * heightOrbit;
            
            movingTargetPos += dronVelocity * velocityOffset;

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
            // Gizmos.DrawLine(dron.transform.position, targetPos);
            //
            // Gizmos.color = Color.red;
            // Gizmos.DrawSphere(targetPos, 0.02f);
            //
            // Gizmos.color = Color.cyan;
            // Gizmos.DrawSphere(dron.transform.position + dron.accelerometer.Velocity * velocityOffset, 0.02f);
        }

        #endregion
    }
}

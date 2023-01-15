using System;
using UnityEngine;

namespace DroneSim
{
    public class Accelerometer : MonoBehaviour
    {
        private DroneController drone;
        private Rigidbody rb;

        [HideInInspector]
        public Vector3 acceleration = Vector3.zero;
        [HideInInspector]
        public Vector3 angularAcceleration = Vector3.zero;
    
        private Vector3 prevVelocity = Vector3.zero;
        private Vector3 prevAngularVelocity = Vector3.zero;

        // Local Velocity Vector
        public Vector3 Velocity => rb.velocity;
        public Vector3 HorizontalVelocity => Quaternion.Euler(0, -drone.gyro.EulerRotation.y, 0) * rb.velocity;
        public Vector3 LocalVelocity => transform.worldToLocalMatrix * rb.velocity;

        public Vector3 AngularVelocity => rb.angularVelocity;
        public Vector3 LocalAngularVelocity => transform.worldToLocalMatrix * rb.angularVelocity;
        

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            drone = GetComponent<DroneController>();
        }

        private void FixedUpdate()
        {
            float deltaTime = Time.inFixedTimeStep ? Time.fixedDeltaTime : Time.deltaTime;
        
            // Velocity Vector
            Vector3 velocity = rb.velocity;
            acceleration = (velocity - prevVelocity) / deltaTime;
        

            Vector3 angularVelocity = rb.angularVelocity;
            angularAcceleration = (angularVelocity - prevAngularVelocity) / deltaTime;


            prevVelocity = velocity;
            prevAngularVelocity = angularVelocity;
        }
    }

}
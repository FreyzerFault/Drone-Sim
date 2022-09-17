using System;
using UnityEngine;

namespace DroneSim
{
    public class Accelerometer : MonoBehaviour
    {
        private Rigidbody rb;

        [HideInInspector]
        public Vector3 acceleration = Vector3.zero;
        [HideInInspector]
        public Vector3 angularAcceleration = Vector3.zero;
    
        private Vector3 prevVelocity = Vector3.zero;
        private Vector3 prevAngularVelocity = Vector3.zero;

        // Local Velocity Vector
        public Vector3 LocalVelocity => transform.worldToLocalMatrix * rb.velocity;
        public Vector3 Velocity => rb.velocity;
        public Vector3 AngularVelocity => rb.angularVelocity;
        

        public Debugging debuggingUI;
    
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            DebuggingUpdate();
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

        #region Debug

        
        private void DebuggingUpdate()
        {
            Vector3 velocity = Quaternion.Euler(0, -transform.rotation.eulerAngles.y.normalizeAngle(), 0) * Velocity;
            debuggingUI.LiftSpeed = Velocity.y;
            debuggingUI.HorizontalSpeedX = velocity.x;
            debuggingUI.HorizontalSpeedZ = velocity.z;
            debuggingUI.YawSpeed = AngularVelocity.y;
        }

        #endregion
    }

}
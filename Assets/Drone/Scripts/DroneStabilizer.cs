using System;
using System.Collections.Generic;
using UnityEngine;

namespace DroneSim
{
    public class DroneStabilizer : MonoBehaviour
    {
        private DroneController drone;
    
        private Gyroscope gyro;
        private Accelerometer accMeter;
        
        
        public bool stabilizePitchAndRoll = true;
        public bool stabilizeYaw = true;
        public bool stabilizeBreak = true;
        public bool stabilizeLiftVelocity = true;
        
        
        // PID Configurations:
        public PIDlabeled[] PIDConfigurations; // This is for seting up the PID configurations in the inspector.
        
        public Dictionary<String, PID_Configuration> PID;

        public Vector3 Velocity => Quaternion.Euler(0, -gyro.EulerRotation.y, 0) * accMeter.Velocity;
        public Vector3 EulerRotation => gyro.EulerRotation;
        
        public DroneSettingsSO DroneSettings => drone.droneSettings;
    
        private void Awake()
        {
            drone = GetComponent<DroneController>();

            accMeter = GetComponent<Accelerometer>();
            gyro = GetComponent<Gyroscope>();
            
            SetupPIDs();
        }

        private bool isReseted;
        public void StabilizeParams(Vector3 targetRotation, Vector3 targetVelocity, Vector3 targetAngularVelocity, ref float lift, ref float pitch, ref float roll, ref float yaw)
        {
            // LIFT Speed Correction
            if (stabilizeLiftVelocity)
            {
                lift = GetPIDcorrection(
                    "ySpeed",
                    targetVelocity.y,
                    Velocity.y, 
                    DroneSettings.saturationValues.maxLiftSpeed
                    );

                if (targetVelocity.x == 0 && targetVelocity.z == 0 && !isReseted)
                {
                    PID["ySpeed"].Values.integralErrorSum = 0;
                    isReseted = true;
                }
                else
                    isReseted = false;
            }
            else
                lift = targetVelocity.y / DroneSettings.saturationValues.maxLiftSpeed;
            
            // YAW Speed Correction
            if (stabilizeYaw)
                yaw = GetPIDcorrection(
                    "yawSpeed",
                    targetAngularVelocity.y,
                    accMeter.AngularVelocity.y,
                    DroneSettings.saturationValues.maxAngularSpeed
                );
            else
                yaw = targetAngularVelocity.y / DroneSettings.saturationValues.maxAngularSpeed;

            // PITCH and ROLL Correction
            if (stabilizePitchAndRoll)
            {
                pitch = GetPIDcorrection("pitch", targetRotation.x, EulerRotation.x, DroneSettings.saturationValues.maxPitch);
                roll = GetPIDcorrection("roll", targetRotation.z, -EulerRotation.z, DroneSettings.saturationValues.maxRoll);
            }
            else
            {
                roll = targetVelocity.x / DroneSettings.saturationValues.maxSpeed;
                pitch = targetVelocity.z / DroneSettings.saturationValues.maxSpeed;
            }
            
            // SPEED BREAK Correction
            if (stabilizeBreak && drone.pitchInput == 0 && drone.rollInput == 0)
            {
                // Use Cascade PID Controller:
                // 1º - Get the PID by Speed for Angle
                float xSpeedPID = GetPIDcorrectionSqr("xSpeed", 0, Velocity.x, DroneSettings.saturationValues.maxSpeed);
                float zSpeedPID = GetPIDcorrectionSqr("zSpeed", 0, Velocity.z, DroneSettings.saturationValues.maxSpeed);

                // Output is a target angle
                float targetRoll = xSpeedPID * DroneSettings.saturationValues.maxRoll;
                float targetPitch = zSpeedPID * DroneSettings.saturationValues.maxPitch;
                
                // 2º - Get the PID by Angle for rotor power
                roll = GetPIDcorrection("roll", targetRoll, -EulerRotation.z, DroneSettings.saturationValues.maxRoll);
                pitch = GetPIDcorrection("pitch", targetPitch, EulerRotation.x, DroneSettings.saturationValues.maxPitch);
            }
        }

        public void ToggleStabilization()
        {
            stabilizeBreak = !stabilizeBreak;
            stabilizeYaw = !stabilizeYaw;
            stabilizeLiftVelocity = !stabilizeLiftVelocity;
            stabilizePitchAndRoll = !stabilizePitchAndRoll;
        }
        
        
        
        #region PID
        
        // [roll, pitch] [x,y]
        public Vector2 GetRollPitchCorrection(Vector2 targetRotation)
        {
            float rollError = targetRotation.x + gyro.Roll / DroneSettings.saturationValues.maxRoll;
            float pitchError = targetRotation.y - gyro.Pitch / DroneSettings.saturationValues.maxPitch;

            return new Vector2(PID_Controller.GetPIDresult(PID["roll"], rollError), PID_Controller.GetPIDresult(PID["pitch"], pitchError));
        }
        
        public Vector2 GetRollPitchCorrectionBySpeed(Vector2 targetSpeed)
        {
            Vector3 velocity = accMeter.Velocity;
            
            velocity = Quaternion.Euler(0, -gyro.EulerRotation.y, 0) * velocity;
            
            float xSpeedError = targetSpeed.x - velocity.x;
            float zSpeedError = targetSpeed.y - velocity.z;

            return new Vector2(PID_Controller.GetPIDresult(PID["xSpeed"], xSpeedError), PID_Controller.GetPIDresult(PID["zSpeed"], zSpeedError));
        }
        
        public float GetPitchCorrection() {
            
            float pitchError = gyro.Pitch / DroneSettings.saturationValues.maxPitch;
            
            float pitchCorrection = -PID_Controller.GetPIDresult(PID["pitch"], pitchError * pitchError * (pitchError > 0 ? 1 : -1)) / 2;

            return pitchCorrection;
        }


        public float GetRollCorrection()
        {
            float rollError = gyro.Roll / DroneSettings.saturationValues.maxRoll;
            
            float rollCorrection = PID_Controller.GetPIDresult(PID["roll"], rollError * rollError * (rollError > 0 ? 1 : -1))/ 2;

            return rollCorrection;
        }
        

        public float GetYawCorrection(float targetAngularSpeed)
        {
            PID_Configuration pid = PID["yawSpeed"];
            
            float angularSpeed = accMeter.AngularVelocity.y;
            
            float angSpeedError = targetAngularSpeed - angularSpeed;

            return PID_Controller.GetPIDresult(pid, angSpeedError);
        }


        public float GetLiftCorrection(float targetLiftSpeed)
        {
            PID_Configuration pid = PID["ySpeed"];
            
            // Try to break velocity to 0 -> error = Velocity Horizontal
            float liftSpeed = accMeter.Velocity.y;
            
            float ySpeedError = targetLiftSpeed - liftSpeed;

            return PID_Controller.GetPIDresult(pid, ySpeedError);
            // return PID_Controller.GetPID(PID["ySpeed"], ySpeedError) * (1 + gyro.Pitch / DroneSettings.saturationValues.maxPitch + gyro.Roll / DroneSettings.saturationValues.maxRoll);
        }
        
        /// <summary>
        /// Get the PID value of an error between target and current value
        /// </summary>
        /// <param name="pidName">PID configuration name</param>
        /// <param name="target">value you need</param>
        /// <param name="current">current value</param>
        /// <param name="maxValue">max value your param can reach</param>
        /// <returns>PID correction in [-1,1]</returns>
        public float GetPIDcorrection(String pidName, float target, float current, float maxValue = 1)
        {
            float error = (target - current) / maxValue;

            return PID_Controller.GetPIDresult(PID[pidName], error);
        }
        
        public float GetPIDcorrectionSqr(String pidName, float target, float current, float maxValue = 1)
        {
            float error = (target - current) / maxValue;
            
            error *= error * (error > 0 ? 1 : -1);

            return PID_Controller.GetPIDresult(PID[pidName], error);
        }

        
        // Setup PID Configurations by name
        private void SetupPIDs()
        {
            PID = new Dictionary<string, PID_Configuration>();
            foreach (PIDlabeled pid in PIDConfigurations)
            {
                PID.Add(pid.name, pid.config);
                PID_Configuration config = PID[pid.name];
                
                // Reset values
                config.ResetValues();
            }
        }
        
        #endregion

    }
}

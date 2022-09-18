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
                StabilizeLift(targetVelocity.y, ref lift);

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
                StabilizeYaw(targetAngularVelocity.y, ref yaw);
            else
                yaw = targetAngularVelocity.y / DroneSettings.saturationValues.maxAngularSpeed;


            // SPEED BREAK Correction to 0
            if (drone.pitchInput == 0 && drone.rollInput == 0 && stabilizeBreak)
            {
                // Use Cascade PID Controller:
                // Get the PID by Speed [X,Z] -> get Angle PID [Roll,Pitch]
                Vector2 speedPID = StabilizeSpeedH(Vector2.zero);

                // Output is a target angle, overwrite the input
                targetRotation = new Vector3(
                    speedPID.y * DroneSettings.saturationValues.maxPitch,   // Pitch
                    targetRotation.y,                                           // Yaw
                    speedPID.x * DroneSettings.saturationValues.maxRoll     // Roll
                );
            }
            
            // PITCH & ROLL Correction
            if (stabilizePitchAndRoll)
            {
                StabilizeRollPitch(targetRotation, ref roll, ref pitch);
            }
            else
            {
                roll = targetRotation.z / DroneSettings.saturationValues.maxRoll;
                pitch = targetRotation.x / DroneSettings.saturationValues.maxPitch;
            }

        }

        private void StabilizeRollPitch(Vector3 targetRotation, ref float roll, ref float pitch)
        {
            pitch = GetPIDcorrection("pitch", targetRotation.x, EulerRotation.x, DroneSettings.saturationValues.maxPitch);
            roll = GetPIDcorrection("roll", targetRotation.z, -EulerRotation.z, DroneSettings.saturationValues.maxRoll);
        }

        private void StabilizeYaw(float targetAngularSpeedY, ref float yaw)
        {
            yaw = GetPIDcorrection(
                "yawSpeed",
                targetAngularSpeedY,
                accMeter.AngularVelocity.y,
                DroneSettings.saturationValues.maxAngularSpeed
            );
        }
        
        private void StabilizeLift(float targetLiftSpeed, ref float lift)
        {
            lift = GetPIDcorrection(
                "ySpeed",
                targetLiftSpeed,
                Velocity.y, 
                DroneSettings.saturationValues.maxLiftSpeed
            );
        }

        private Vector2 StabilizeSpeedH(Vector2 targetSpeed)
        {
            return new Vector2(
                GetPIDcorrectionSqr("xSpeed", targetSpeed.x, Velocity.x, DroneSettings.saturationValues.maxSpeed),
                GetPIDcorrectionSqr("zSpeed", targetSpeed.y, Velocity.z, DroneSettings.saturationValues.maxSpeed)
                );
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
            // / 2 to get the diff in [-1,1] => [-1,1]
            float error = (target - current) / maxValue / 2;

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


using System;
using System.Collections.Generic;
using UnityEngine;

namespace DroneSim
{
    public class DroneStabilizer : MonoBehaviour
    {
        private DroneController drone;

        public Gyroscope Gyro => drone.gyro;
        public Accelerometer AccMeter => drone.accelerometer;

        public bool stabilizePitchAndRoll = true;
        public bool stabilizeYaw = true;
        public bool stabilizeBreak = true;
        public bool stabilizeLiftVelocity = true;
        
        
        // PID Configurations:
        public PIDlabeled[] PIDConfigurations; // This is for seting up the PID configurations in the inspector.
        
        public Dictionary<String, PID_Configuration> PID;

        public Vector3 Velocity => Quaternion.Euler(0, -Gyro.EulerRotation.y, 0) * AccMeter.Velocity;
        public Vector3 EulerRotation => Gyro.EulerRotation;
        
        public DroneSettingsSO DroneSettings => drone.droneSettings;
    
        private void Awake()
        {
            drone = GetComponent<DroneController>();

            
            SetupPIDs();
        }

        private bool isReseted;

        // Control pitch and roll to target an Angle Of Attack (X = Pitch, Y = Roll)
        public Vector2 StabilizeAngleOfAttack(Vector2 targetAngle)
        {
            Vector2 currentAngle = new Vector2(EulerRotation.x, EulerRotation.z);
            float pitch = GetPIDcorrection("pitch", targetAngle.x, currentAngle.x, DroneSettings.saturationValues.maxAngleOfAttack);
            float roll = GetPIDcorrection("roll", targetAngle.y, -currentAngle.y, DroneSettings.saturationValues.maxAngleOfAttack);

            return new Vector2(pitch, roll);
        }
        
        // Control pitch, roll and yaw to target an Angular Velocity (X = Pitch, Y = Yaw, Z = Roll)
        public Vector3 StabilizeAngularVelocity(Vector3 targetAngularVelocity)
        {
            float responseTime = 1; // in Seconds
            
            Vector3 currentAngVel = AccMeter.LocalAngularVelocity;
            float pitch = GetPIDcorrection("pitch", targetAngularVelocity.x, currentAngVel.x, DroneSettings.saturationValues.maxAngularSpeed);
            float yaw = GetPIDcorrection("yaw", targetAngularVelocity.y, currentAngVel.y, DroneSettings.saturationValues.maxAngularSpeed);
            float roll = GetPIDcorrection("roll", targetAngularVelocity.z, -currentAngVel.z, DroneSettings.saturationValues.maxAngularSpeed);

            Vector3 angularVelCorrection = new Vector3(pitch, yaw, roll) * DroneSettings.saturationValues.maxAngularSpeed;
            
            // Para pasar Velocidad Angular a la Fuerza que hay que aplicar en los motores, aplicamos formulas de dinamica:
            // Velocidad Angular: w = vR
            // Fuerza: F = ma = mv/t = mwR/t
            // Asi se podra conseguir una Velocidad Angular correcta independientemente de la masa y el radio del dron.
            Vector3 angularForce = angularVelCorrection * (drone.Mass * drone.Radius / responseTime);

            // Max angular force = 2 rotores al maximo
            angularForce /= DroneSettings.saturationValues.maxThrottle / 2;
            angularForce = new Vector3(Mathf.Clamp(angularForce.x, -1, 1), Mathf.Clamp(angularForce.y, -1, 1),
                Mathf.Clamp(angularForce.z, -1, 1));

            return angularForce;
        }
        
        public void StabilizeParams(Vector3 targetAngle, Vector3 targetVelocity, Vector3 targetAngularVelocity, ref float lift, ref float pitch, ref float roll, ref float yaw)
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
                lift = drone.Curves.liftCurve.Evaluate(drone.liftInput);
            
            // YAW Speed Correction
            if (stabilizeYaw)
                StabilizeYaw(targetAngularVelocity.y, ref yaw);
            else
                yaw = drone.Curves.yawCurve.Evaluate(drone.yawInput);
                // yaw = targetAngularVelocity.y / DroneSettings.saturationValues.maxAngularSpeed;


            // SPEED BREAK Correction to 0
            if (drone.pitchInput == 0 && drone.rollInput == 0 && stabilizeBreak)
            {
                // Use Cascade PID Controller:
                // Get the PID by Speed [X,Z] -> get Angle PID [Roll,Pitch]
                Vector2 speedPID = StabilizeSpeedH(Vector2.zero);

                // Output is a target angle, overwrite the input
                targetAngle = new Vector3(
                    speedPID.y * DroneSettings.saturationValues.maxAngleOfAttack,   // Pitch
                    targetAngle.y,                                                  // Yaw
                    speedPID.x * DroneSettings.saturationValues.maxAngleOfAttack    // Roll
                );
            }
            
            // PITCH & ROLL Correction
            if (stabilizePitchAndRoll)
            {
                StabilizeAngleOfAttack(targetAngle, ref roll, ref pitch);
            }
            else
            {
                pitch = drone.Curves.liftCurve.Evaluate(drone.pitchInput);
                roll = drone.Curves.liftCurve.Evaluate(drone.rollInput);
            }

        }

        private void StabilizeAngleOfAttack(Vector3 targetRotation, ref float roll, ref float pitch)
        {
            pitch = GetPIDcorrection("pitch", targetRotation.x, EulerRotation.x, DroneSettings.saturationValues.maxAngleOfAttack);
            roll = GetPIDcorrection("roll", targetRotation.z, -EulerRotation.z, DroneSettings.saturationValues.maxAngleOfAttack);
        }

        private void StabilizeYaw(float targetAngularSpeedY, ref float yaw)
        {
            yaw = GetPIDcorrection(
                "yaw",
                targetAngularSpeedY,
                AccMeter.AngularVelocity.y,
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
            float rollError = targetRotation.x + Gyro.Roll / DroneSettings.saturationValues.maxAngleOfAttack;
            float pitchError = targetRotation.y - Gyro.Pitch / DroneSettings.saturationValues.maxAngleOfAttack;

            return new Vector2(PID_Controller.GetPIDresult(PID["roll"], rollError), PID_Controller.GetPIDresult(PID["pitch"], pitchError));
        }
        
        public Vector2 GetRollPitchCorrectionBySpeed(Vector2 targetSpeed)
        {
            Vector3 velocity = AccMeter.Velocity;
            
            velocity = Quaternion.Euler(0, -Gyro.EulerRotation.y, 0) * velocity;
            
            float xSpeedError = targetSpeed.x - velocity.x;
            float zSpeedError = targetSpeed.y - velocity.z;

            return new Vector2(PID_Controller.GetPIDresult(PID["xSpeed"], xSpeedError), PID_Controller.GetPIDresult(PID["zSpeed"], zSpeedError));
        }
        
        public float GetPitchCorrection() {
            
            float pitchError = Gyro.Pitch / DroneSettings.saturationValues.maxAngleOfAttack;
            
            float pitchCorrection = -PID_Controller.GetPIDresult(PID["pitch"], pitchError * pitchError * (pitchError > 0 ? 1 : -1)) / 2;

            return pitchCorrection;
        }


        public float GetRollCorrection()
        {
            float rollError = Gyro.Roll / DroneSettings.saturationValues.maxAngleOfAttack;
            
            float rollCorrection = PID_Controller.GetPIDresult(PID["roll"], rollError * rollError * (rollError > 0 ? 1 : -1))/ 2;

            return rollCorrection;
        }
        

        public float GetYawCorrection(float targetAngularSpeed)
        {
            PID_Configuration pid = PID["yawSpeed"];
            
            float angularSpeed = AccMeter.AngularVelocity.y;
            
            float angSpeedError = targetAngularSpeed - angularSpeed;

            return PID_Controller.GetPIDresult(pid, angSpeedError);
        }


        public float GetLiftCorrection(float targetLiftSpeed)
        {
            PID_Configuration pid = PID["ySpeed"];
            
            // Try to break velocity to 0 -> error = Velocity Horizontal
            float liftSpeed = AccMeter.Velocity.y;
            
            float ySpeedError = targetLiftSpeed - liftSpeed;

            return PID_Controller.GetPIDresult(pid, ySpeedError);
            // return PID_Controller.GetPID(PID["ySpeed"], ySpeedError) * (1 + Gyro.Pitch / DroneSettings.saturationValues.maxPitch + Gyro.Roll / DroneSettings.saturationValues.maxRoll);
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
            // error / max * 2 to get the diff in [-max, max] => [-1,1]
            float error = (target - current) / (maxValue * 2);

            return PID_Controller.GetPIDresult(PID[pidName], error);
        }
        
        public float GetPIDcorrectionSqr(String pidName, float target, float current, float maxValue = 1)
        {
            // error / max * 2 to get the diff in [-max, max] => [-1,1]
            float error = (target - current) / (maxValue * 2);
            
            error *= Math.Abs(error);

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


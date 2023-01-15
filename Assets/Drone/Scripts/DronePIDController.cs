using System;
using System.Collections.Generic;
using UnityEngine;

namespace DroneSim
{
    public class DronePIDController : MonoBehaviour
    {
        private DroneController drone;

        public Gyroscope Gyro => drone.gyro;
        public Accelerometer AccMeter => drone.accelerometer;

        // PID Configurations:
        public PIDlabeled[] PIDConfigurations; // This is for seting up the PID configurations in the inspector.

        private Dictionary<String, PID_Configuration> PIDConfigurationMap;
        private Dictionary<String, float> ErrorsLastFrame;
        private Dictionary<String, float> ErrorsSum;

        private DroneSettingsSO.SaturationValues SaturationValues => drone.droneSettings.saturationValues;
    
        private void Awake()
        {
            drone = GetComponent<DroneController>();
            
            SetupPIDs();
        }

        // Control pitch and roll to target an Angle Of Attack (X = Pitch, Y = Roll)
        public Vector2 AdjustAngleOfAttack(Vector2 targetAngle)
        {
            Vector2 currentAngle = new Vector2(Gyro.EulerRotation.x, Gyro.EulerRotation.z);
            float maxAngle = SaturationValues.maxAngleOfAttack;
            float pitch = GetPIDcorrection("pitch", targetAngle.x, currentAngle.x, maxAngle);
            float roll = GetPIDcorrection("roll", targetAngle.y, -currentAngle.y, maxAngle);

            return new Vector2(pitch, roll);
        }
        
        public Vector3 AdjustAngleOfAttack(Vector3 targetAngle)
        {
            Vector2 out2D = AdjustAngleOfAttack(new Vector2(targetAngle.x, targetAngle.z));
            return new Vector3(out2D.x, 0, out2D.y);
        }

        // Control pitch, roll and yaw to target an Angular Velocity (X = Pitch, Y = Yaw, Z = Roll)
        public Vector3 AdjustAngularVelocity(Vector3 targetAngularVelocity)
        {
            float responseTime = 1; // in Seconds
            
            Vector3 currentAngVel = AccMeter.LocalAngularVelocity;
            float pitch = GetPIDcorrection("pitch", targetAngularVelocity.x, currentAngVel.x, SaturationValues.maxAngularSpeed);
            float yaw = GetPIDcorrection("yaw", targetAngularVelocity.y, currentAngVel.y, SaturationValues.maxAngularSpeed);
            float roll = GetPIDcorrection("roll", targetAngularVelocity.z, -currentAngVel.z, SaturationValues.maxAngularSpeed);

            Vector3 angularVelCorrection = new Vector3(pitch, yaw, roll) * SaturationValues.maxAngularSpeed;
            
            // Para pasar Velocidad Angular a la Fuerza que hay que aplicar en los motores, aplicamos formulas de dinamica:
            // Velocidad Angular: w = vR
            // Fuerza: F = ma = mv/t = mwR/t
            // Asi se podra conseguir una Velocidad Angular correcta independientemente de la masa y el radio del dron.
            Vector3 angularForce = angularVelCorrection * (drone.Mass * drone.Radius / responseTime);

            // Max angular force = 2 rotores al maximo
            angularForce /= SaturationValues.maxThrottle / 2;
            angularForce = new Vector3(Mathf.Clamp(angularForce.x, -1, 1), Mathf.Clamp(angularForce.y, -1, 1),
                Mathf.Clamp(angularForce.z, -1, 1));

            return angularForce;
        }

        // Ajusta el yaw para conseguir una velocidad angular en el eje Y objetivo
        public float AdjustYaw(float targetAngularSpeedY)
        {
            return GetPIDcorrection(
                "yaw",
                targetAngularSpeedY,
                AccMeter.LocalAngularVelocity.y,
                SaturationValues.maxAngularSpeed
            );
        }
        
        // Ajusta el lift para conseguir una velocidad vertical en el eje Y objetivo
        public float AdjustLift(float targetLiftSpeed)
        {
            return GetPIDcorrection(
                "ySpeed",
                targetLiftSpeed,
                AccMeter.Velocity.y,
                SaturationValues.maxLiftSpeed
            );
        }

        // Ajusta el pitch y roll para conseguir una velocidad horizontal objetivo
        public Vector2 AdjustHorizontalSpeed(Vector2 targetSpeed)
        {
            return new Vector2(
                GetPIDcorrection("xSpeed", targetSpeed.x, AccMeter.HorizontalVelocity.x, SaturationValues.maxSpeed),
                GetPIDcorrection("zSpeed", targetSpeed.y, AccMeter.HorizontalVelocity.z, SaturationValues.maxSpeed)
                );
        }



        #region PID

        /// <summary>
        /// Get the PID value of an error between target and current value
        /// </summary>
        /// <param name="pidName">PID configuration name</param>
        /// <param name="target">value you need</param>
        /// <param name="current">current value</param>
        /// <param name="maxValue">max value your param can reach</param>
        /// <returns>PID correction in [-1,1]</returns>
        private float GetPIDcorrection(String pidName, float target, float current, float maxValue = 1)
        {
            // error / max * 2 to get the diff in [-max, max] => [-1,1]
            float error = (target - current) / (maxValue * 2);
            
            float lastFrameError = ErrorsLastFrame[pidName];
            float errorSum = PID.antiWindupFilter(ErrorsSum[pidName] + error, error);
            
            float correction = PID.GetPIDresult(PIDConfigurationMap[pidName], error, errorSum, lastFrameError);

            ErrorsLastFrame[pidName] = error;
            ErrorsSum[pidName] += errorSum == 0 ? 0 : error;

            return correction;
        }
        
        private float GetPIDcorrectionSqr(String pidName, float target, float current, float maxValue = 1)
        {
            // error / max * 2 to get the diff in [-max, max] => [-1,1]
            float error = (target - current) / (maxValue * 2);
            
            float lastFrameError = ErrorsLastFrame[pidName];
            float errorSum = PID.antiWindupFilter(ErrorsSum[pidName] + error, error);
            
            float correction = PID.GetPIDresult(PIDConfigurationMap[pidName], errorSum, error * Math.Abs(error), lastFrameError);

            ErrorsLastFrame[pidName] = error;
            ErrorsSum[pidName] += errorSum == 0 ? 0 : error;

            return correction;
        }

        
        // Setup PID Configurations by name
        private void SetupPIDs()
        {
            PIDConfigurationMap = new Dictionary<String, PID_Configuration>();
            ErrorsLastFrame = new Dictionary<String, float>();
            ErrorsSum = new Dictionary<string, float>();
            
            foreach (PIDlabeled pid in PIDConfigurations)
            {
                PIDConfigurationMap.Add(pid.name, pid.config);
                ErrorsLastFrame.Add(pid.name, 0);
                ErrorsSum.Add(pid.name, 0);
                
                PID_Configuration config = PIDConfigurationMap[pid.name];
            }
        }
        
        #endregion

    }
}

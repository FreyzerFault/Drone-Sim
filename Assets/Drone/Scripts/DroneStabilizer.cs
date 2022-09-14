using UnityEngine;

namespace DroneSim
{
    public class DroneStabilizer : MonoBehaviour
    {
        private DroneController drone;
    
        private Gyroscope gyro;
        private Accelerometer accMeter;
        
        public PID_Controller PID_pitch;
        public PID_Controller PID_roll;
        public PID_Controller PID_yaw;

        public DroneSettingsSO DroneSettings => drone.droneSettings;
    
        private void Awake()
        {
            drone = GetComponent<DroneController>();

            accMeter = GetComponent<Accelerometer>();
            gyro = GetComponent<Gyroscope>();
            gyro.maxPitch = this.DroneSettings.saturationValues.maxPitch;
            gyro.maxRoll = this.DroneSettings.saturationValues.maxRoll;
            
            PID_pitch.Reset();
            PID_roll.Reset();
            PID_yaw.Reset();
        }
        

        #region PID

        public float GetPitchCorrection() {
            
            float pitchError = gyro.PitchError / DroneSettings.saturationValues.maxPitch;
            
            float pitchCorrection = -PID_pitch.GetPID(pitchError * pitchError * (pitchError > 0 ? 1 : -1)) / 2;

            return pitchCorrection;
        }

        public float GetRollCorrection()
        {
            float rollError = gyro.RollError / this.DroneSettings.saturationValues.maxRoll;
            
            float rollCorrection = PID_roll.GetPID(rollError * rollError * (rollError > 0 ? 1 : -1)) / 2;

            return rollCorrection;
        }

        public float GetYawCorrection()
        {
            float angularVelY = accMeter.AngularVelocity.y / DroneSettings.saturationValues.maxTorque * 4;

            float fixYaw = -PID_yaw.GetPID(angularVelY);

            return fixYaw;
        }

        #endregion
    }
}


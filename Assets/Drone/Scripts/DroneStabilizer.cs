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
        public PID_Controller PID_breakPitch;
        public PID_Controller PID_breakRoll;
        public PID_Controller PID_height;

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
            float rollError = gyro.RollError / DroneSettings.saturationValues.maxRoll;
            
            float rollCorrection = PID_roll.GetPID(rollError * rollError * (rollError > 0 ? 1 : -1)) / 2;

            return rollCorrection;
        }

        public float GetYawCorrection()
        {
            float angularVelY = accMeter.AngularVelocity.y / DroneSettings.saturationValues.maxTorque * 4;

            float fixYaw = -PID_yaw.GetPID(angularVelY);

            return fixYaw;
        }

        public Vector2 GetBreakPitchRoll()
        {
            // Try to break velocity to 0 -> error = Velocity Horizontal
            Vector3 velocity = accMeter.Velocity / DroneSettings.parameters.maxSpeed;

            float pitchError = velocity.z * velocity.z * (velocity.z > 0 ? 1 : -1);
            float rollError = velocity.x * velocity.x * (velocity.x > 0 ? 1 : -1);
            

            float pitchCorrection = -PID_breakPitch.GetPID(pitchError) / DroneSettings.saturationValues.maxPitch;
            float rollCorrection = -PID_breakRoll.GetPID(rollError) / DroneSettings.saturationValues.maxRoll;

            return new Vector2(pitchCorrection, rollCorrection);
        }

        public float GetHeightCorrection()
        {
            // Try to break velocity to 0 -> error = Velocity Horizontal
            Vector3 velocity = accMeter.Velocity;

            float heightError = velocity.y * velocity.y * (velocity.y > 0 ? 1 : -1) / 10;

            float heightCorrection = -PID_height.GetPID(heightError);
            
            //Debug.Log("Height Correction: " + heightCorrection);

            return heightCorrection;
        }

        #endregion
    }
}


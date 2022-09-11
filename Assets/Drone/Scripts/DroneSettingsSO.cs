using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Drone Settings", menuName = "Settings/Drone Settings", order = 1)]
public class DroneSettingsSO : ScriptableObject
{
    [Serializable]
    // Limit Physic values
    public class SaturationValues {
        // Max params of the propellers
        public float maxRotationSpeed = 10000;
        public float maxTorque = 1;
        
        // Depends on drone's surface facing velocity direction (angle of attack)
        public float minDragCoefficient = 0.35f;
        public float maxDragCoefficient = 0.80f;
        
        // Movement params
        public float maxPitch = 30;
        public float maxRoll = 30;
        public float yawPower = 4;
    }
    
    public SaturationValues saturationValues = new SaturationValues();
}

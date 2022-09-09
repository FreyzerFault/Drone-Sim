using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Drone Settings", menuName = "Settings/Drone Settings", order = 1)]
public class DroneSettingsSO : ScriptableObject
{
    [Serializable]
    // Limit Physic values
    public class SaturationValues {
        // Max rotation speed of the propellers
        public float MaxRotationSpeed = 3f;
        
        // Depends on drone's surface facing velocity direction (angle of attack)
        public float MinDragCoefficient = 0.01f;
        public float MaxDragCoefficient = 0.25f;
    }
    
    public SaturationValues saturationValues = new SaturationValues();
}

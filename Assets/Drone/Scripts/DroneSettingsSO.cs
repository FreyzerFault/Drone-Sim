using System;
using UnityEngine;

namespace DroneSim
{
    [CreateAssetMenu(fileName = "Drone Settings", menuName = "Settings/Drone Settings", order = 1)]

    public class DroneSettingsSO : ScriptableObject
    {
        [Serializable]
        // Limit Physic values
        public class SaturationValues
        {
            // Max params of the propellers
            public float maxPower = 1;
            public float maxRotationSpeed = 10000;
            public float maxTorque = 1;
            public float maxThrottle = 1;


            // Movement params
            public float maxAngleOfAttack = 60;
            public float maxLift = 5;
            public float yawPower = 4;
            
            // Speed params
            public float maxSpeed = 10;
            public float maxLiftSpeed = 10;
            public float maxAngularSpeed = 10;
        }

        [Serializable]
        public class DroneParams
        {
            public float mass = 1;

            // Depends on drone's surface facing velocity direction (angle of attack)
            public float minDragCoefficient = 0.35f;
            public float maxDragCoefficient = 0.80f;

            public float angularDrag = 0.5f;
        }

        [Serializable]
        public class Curves
        {
            // Por si es necesario separarlas
            // public AnimationCurve pitchCurve;
            // public AnimationCurve rollCurve;
            
            public AnimationCurve pitchRollCurve;
            public AnimationCurve liftCurve;
            public AnimationCurve yawCurve;
        }

        public SaturationValues saturationValues = new SaturationValues();
        public DroneParams parameters = new DroneParams();
        public Curves curves = new Curves();
    }
}

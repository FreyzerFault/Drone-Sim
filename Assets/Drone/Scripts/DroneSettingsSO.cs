using System;
using UnityEngine;

namespace DroneSim
{
    [CreateAssetMenu(fileName = "Drone Settings", menuName = "Settings/Drone Settings", order = 1)]
    public class DroneSettingsSO : ScriptableObject
    {
        [Header("Modelo 3D")] 
        public GameObject prefab;
        public Sprite previewImage;
        
        [Header("Input Curves")]
        public AnimationCurve pitchRollCurve;
        public AnimationCurve liftCurve;
        public AnimationCurve yawCurve;
        
        [Header("Physic Parameters")]
        public float mass = 1;

        // Depends on drone's surface facing velocity direction (angle of attack)
        public float minDragCoefficient = 0.35f;
        public float maxDragCoefficient = 0.80f;

        public float angularDrag = 0.5f;
        
        [Header("Saturation Values")]
        // Max params of the propellers
        public float maxRotationSpeed = 10000;

        public float maxTorque = 1;
        public float maxThrottle = 1;


        // Movement params
        public float maxAngleOfAttack = 60;

        // Speed params
        public float maxSpeed = 10;
        public float maxLiftSpeed = 10;
        public float maxAngularSpeed = 10;
    }
}
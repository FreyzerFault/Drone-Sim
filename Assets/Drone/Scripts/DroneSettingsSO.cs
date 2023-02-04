using System;
using UnityEngine;

namespace DroneSim
{
    [CreateAssetMenu(fileName = "Drone Settings", menuName = "Settings/Drone Settings", order = 1)]
    public class DroneSettingsSO : ScriptableObject
    {
        public string configurationName;
        
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
        public float maxDragCoefficient = 1.00f;

        public float angularDrag = 2f;
        
        [Header("Saturation Values")]
        public float maxTorque = 1;
        public float maxThrottle = 10;


        // Movement params
        public float maxAngleOfAttack = 60;

        // Speed params
        public float maxSpeed = 10;
        public float maxLiftSpeed = 10;
        public float maxAngularSpeed = 30;
        
        // Max params of the propellers animation
        public float maxRotationSpeed = 1000;
    }
}
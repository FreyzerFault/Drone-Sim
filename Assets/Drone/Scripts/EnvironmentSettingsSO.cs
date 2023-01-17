using System;
using UnityEngine;

namespace DroneSim
{
    [CreateAssetMenu(fileName = "Environment Settings", menuName = "Settings/Environment Settings", order = 2)]

    public class EnvironmentSettingsSO : ScriptableObject
    {
        public float gravity = 9.81f;
        
        [Header("Atmospheric Settings")]
        public float airDensity;
        public Vector3 windForce;


        public void ApplySettings()
        {
            UpdateGravity(gravity);
        }

        public static void UpdateGravity(float newGravity) => Physics.gravity = new Vector3(0, -newGravity, 0); 
    }
}

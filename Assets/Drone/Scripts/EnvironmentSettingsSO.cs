using System;
using UnityEngine;

namespace DroneSim
{
    [CreateAssetMenu(fileName = "Environment Settings", menuName = "Settings/Environment Settings", order = 2)]

    public class EnvironmentSettingsSO : ScriptableObject
    {
        [Serializable]
        public class AtmosphericSettings
        {
            public float airDensity;
            public Vector3 windForce;
        }

        public AtmosphericSettings atmosphericSettings;
    }
}

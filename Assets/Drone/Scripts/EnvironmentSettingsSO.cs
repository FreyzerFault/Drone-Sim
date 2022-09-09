using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Environment Settings", menuName = "Settings/Environment Settings", order = 2)]
public class EnvironmentSettingsSO : ScriptableObject
{
    public class AtmosphericSettings
    {
        public float airDensity;
        public Vector3 windForce;
    }
}

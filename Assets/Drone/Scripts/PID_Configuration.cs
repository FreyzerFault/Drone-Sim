using System;
using UnityEngine;

[Serializable]
public struct PIDlabeled
{
    public String name;
    public PID_Configuration config;
}

[CreateAssetMenu(fileName = "PID_", menuName = "PID", order = 1)]
public class PID_Configuration : ScriptableObject
{
    #region Parameters

    // Constants
    public float pGain = 1f;
    public float iGain = 1f;
    public float dGain = 1f;

    #endregion

    public bool useP = true;
    public bool useI = true;
    public bool useD = true;
}
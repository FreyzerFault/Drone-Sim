using System;
using Unity.VisualScripting;
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
    public struct PIDvalues
    {
        public float previousError;
        public float previousError2Frames;
        public float integralErrorSum;
        
        public void Reset() => previousError = previousError2Frames = integralErrorSum = 0;
    }
    
    #region Parameters

    // Constants
    public float pGain = 1f;
    public float iGain = 1f;
    public float dGain = 1f;

    #endregion

    public bool useP = true;
    public bool useI = true;
    public bool useD = true;
    
    public PIDvalues Values;

    public float iSum;
    public float error;

    public void ResetValues() => Values.Reset();
}

public static class PID_Controller
{
    
    // Result is in [-1,1]
    public static float GetPIDresult(PID_Configuration config, float error)
    {
        float deltaTime = Time.inFixedTimeStep ? Time.fixedDeltaTime : Time.deltaTime;
        float p = 0, i = 0, d = 0;
        
        // P -> Proportional Error (Present)
        if (config.useP)
            p = config.pGain * error;

        // I -> Integral Error (Past)
        if (config.useI)
        {
            // Windup Filter
            float sum = antiWindupFilter(config.Values.integralErrorSum + error * deltaTime, error);

            // Acumulamos el error siempre que no se active el filtro de Windup
            if (sum != 0)
            {
                config.Values.integralErrorSum = sum;
                i = config.iGain * sum;
            }

            // =======================================================================
            // DEBUG SUM
            config.iSum = config.Values.integralErrorSum;
            config.error = error;
            // =======================================================================
        }
        
        // D -> Derivative Error (Future)
        if (config.useD)
        {
            d = config.dGain * (error - config.Values.previousError) / deltaTime;
        }

        config.Values.previousError2Frames = config.Values.previousError;
        config.Values.previousError = error;
        
        return Mathf.Clamp(p + i + d, -1, 1);
    }

    // Anti Integral Windup Clamping Filter:
    // Clamp Integral Error if its out of saturation values [-1,1] & its not reducing the error
    public static float antiWindupFilter(float sum, float error)
    {
        bool saturated = sum > 1 || sum < -1;
        bool sameSign = Math.Abs(Mathf.Sign(sum) - Mathf.Sign(error)) < 0.00001f;
        
        return saturated && sameSign ? 0 : sum;
    }
}
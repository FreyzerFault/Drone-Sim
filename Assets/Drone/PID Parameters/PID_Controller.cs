using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PID_", menuName = "PID", order = 1)]
public class PID_Controller : ScriptableObject
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

    private float currentError = 0;
    private float previousError = 0;
    private float previousError2Frames = 0;
    public float integralError = 0;
    
    public float PIDp = 0;
    public float PIDi = 0;
    public float PIDd = 0;

    public float maxIntegralSum = 1;
    
    public void Reset()
    {
        currentError = previousError = previousError2Frames = integralError = 0;
        PIDp = PIDi = PIDd = 0;
    }

    public float GetPID(float error)
    {
        float deltaTime = Time.inFixedTimeStep ? Time.fixedDeltaTime : Time.deltaTime; 
        currentError = error;
        
        // Clamp I SUM to [-max,max] to avoid accumulating too much error
        

        if (useP)
        {
            // Error Presente
            PIDp = pGain * error;
        }
        else PIDp = 0;
        
        // Acumula Ki * error del Pasado
        if (useI)
        {
            // Error Presente
            integralError = Mathf.Clamp(integralError + error * deltaTime, -maxIntegralSum, maxIntegralSum);
            PIDi = iGain * integralError;
        }
        else PIDi = 0;
        
        if (useD)
        {
            // Derivada del Error / Tiempo para predecir el Futuro
            PIDd = dGain * (error - previousError) / deltaTime;
        }
        else PIDd = 0;

        previousError2Frames = previousError;
        previousError = error;
        
        return PIDp + PIDi + PIDd;
    }
}

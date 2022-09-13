using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PID_", menuName = "PID", order = 1)]
public class PID_Controller : ScriptableObject
{
    #region Parameters

    // Constants
    public float Kp = 20f;
    public float Ki = 1f;
    public float Kd = 5000f;

    #endregion

    public bool useP = true;
    public bool useI = true;
    public bool useD = true;

    private float currentError = 0;
    private float previousError = 0;
    private float previousError2Frames = 0;
    private float integralError = 0;
    
    private float PIDp = 0;
    private float PIDi = 0;
    private float PIDd = 0;

    public void Reset()
    {
        currentError = previousError = previousError2Frames = integralError = 0;
        PIDp = PIDi = PIDd = 0;
    }

    public float GetPID(float error)
    {
        float deltaTime = Time.inFixedTimeStep ? Time.fixedDeltaTime : Time.deltaTime; 
        currentError = error;
        
        if (useP)
        {
            // Error Presente
            PIDp = Kp * error;
        }
        else PIDp = 0;
        
        // Acumula Ki * error del Pasado
        if (useI)
        {
            // Error Presente
            integralError += error * deltaTime;
            PIDi = Ki * integralError;
        }
        else PIDp = 0;
        
        if (useD)
        {
            // Derivada del Error / Tiempo para predecir el Futuro
            PIDd = Kd * (error - previousError) / deltaTime;
        }
        else PIDp = 0;

        previousError2Frames = previousError;
        previousError = error;
        
        return PIDp + PIDi + PIDd;
    }
}

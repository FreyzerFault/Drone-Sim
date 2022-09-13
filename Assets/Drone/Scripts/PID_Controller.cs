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

    private float currentError;
    private float previousError;
    private float previousError2Frames;
    private float integralError;
    
    private float PIDp;
    private float PIDi;
    private float PIDd;

    public float GetPID(float error)
    {
        float deltaTime = Time.inFixedTimeStep ? Time.fixedDeltaTime : Time.deltaTime; 
        currentError = error;
        
        // Error Presente
        PIDp = Kp * error;
        
        // Acumula Ki * error del Pasado
        PIDi += Ki * error;
        
        // Derivada del Error / Tiempo para predecir el Futuro
        PIDd = Kd * (error - previousError) / deltaTime;
        
        return PIDp + PIDi + PIDd;
    }
}

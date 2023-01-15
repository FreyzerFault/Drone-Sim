using UnityEngine;

// Esta clase reune todos los algoritmos PID
public static class PID
{
    // Result is in [-1,1]
    public static float GetPIDresult(PID_Configuration config, float error, float errorSum, float lastFrameError)
    {
        if (error > 1 || error < -1)
        {
            Debug.LogError("Error is out of range [-1,1]: " + config.name + " = " + error);
            return 0;
        }

        float deltaTime = Time.inFixedTimeStep ? Time.fixedDeltaTime : Time.deltaTime;
        float p = 0, i = 0, d = 0;

        // P -> Proportional Error (Present)
        if (config.useP)
            p = config.pGain * error;

        // I -> Integral Error (Past)
        if (config.useI)
        {
            // Acumulamos el error siempre que no se active el filtro de Windup
            i = config.iGain * errorSum;
        }

        // D -> Derivative Error (Future)
        if (config.useD)
        {
            d = config.dGain * (error - lastFrameError) / deltaTime;
        }

        return Mathf.Clamp(p + i + d, -1, 1);
    }

    // Anti Integral Windup Clamping Filter:
    // Clamp Integral Error if its out of saturation values [-1,1] & its not reducing the error
    public static float antiWindupFilter(float sum, float error)
    {
        bool saturated = sum > 1 || sum < -1;
        bool sameSign = Mathf.Abs(Mathf.Sign(sum) - Mathf.Sign(error)) < 0.00001f;

        return saturated && sameSign ? 0 : sum;
    }
}
using UnityEngine;

/**
 * Esta clase reune todos los algoritmos PID
 */
public static class PID
{
    // Result is in [-1,1]
    public static float GetPIDresult(PID_Configuration config, float error, float errorSum, float lastFrameError)
    {
        if (error is > 1 or < -1)
        {
            Debug.LogError("Error is out of range [-1,1]: " + config.name + " = " + error);
            return 0;
        }

        float deltaTime = Time.inFixedTimeStep ? Time.fixedDeltaTime : Time.deltaTime;

        float p = config.useP ? Proportional(config.pGain, error) : 0;
        float i = config.useI ? Integral(config.iGain, error, errorSum) : 0;
        float d = config.useD ? Derivative(config.dGain, error, lastFrameError, deltaTime) : 0;

        return Mathf.Clamp(p + i + d, -1, 1);
    }


    // P -> Proportional Error (Present)
    private static float Proportional(float pGain, float error) => pGain * error;

    // I -> Integral Error (Past)
    private static float Integral(float iGain, float error, float errorSum)
        => iGain * antiWindupFilter(errorSum, error);

    // D -> Derivative Error (Future)
    private static float Derivative(float dGain, float error, float lastError, float deltaTime)
        => dGain * (error - lastError) / deltaTime;

    // Anti Integral Windup Clamping Filter:
    // Clamp Integral Error if its out of saturation values [-1,1] & its not reducing the error
    public static float antiWindupFilter(float sum, float error)
    {
        bool saturated = sum > 1 || sum < -1;
        bool sameSign = Mathf.Abs(Mathf.Sign(sum) - Mathf.Sign(error)) < 0.00001f;

        return saturated && sameSign ? 0 : sum;
    }
}
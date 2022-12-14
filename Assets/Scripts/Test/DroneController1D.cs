using UnityEditor;
using UnityEngine;

public class DroneController1D : MonoBehaviour
{
    public enum FlightMode{Angle, Horizon, Manual}
    
    public FlightMode flightMode;
    public bool hoverStabilization = true;
    
    private Rigidbody rb;

    public Transform r1, r2, r3, r4;

    public float radius = 1;
    public float mass = 1;
    
    public float maxRotorForce = 40;
    public float maxRotationalForce = 10;
    public float maxLiftForce = 10;
    public float maxTorque = 10;
    [Range(-1,1)]public float pitchPower;
    [Range(-1,1)]public float rollPower;
    [Range(-1,1)]public float yawPower;
    [Range(-1,1)]public float liftPower;

    public float NormalizedPitchPower => RotationCurve.Evaluate(pitchPower);
    public float NormalizedRollPower => RotationCurve.Evaluate(rollPower);
    public float NormalizedYawPower => RotationCurve.Evaluate(yawPower);
    public float NormalizedLiftPower => LiftCurve.Evaluate(liftPower);

    public float PitchForce => NormalizedPitchPower * maxRotationalForce;
    public float RollForce => NormalizedRollPower * maxRotationalForce;
    public float YawForce => NormalizedYawPower * maxRotationalForce;
    public float LiftForce => NormalizedLiftPower * maxLiftForce;

    public float PitchMultiplier => (NormalizedPitchPower + 1) / 2;
    public float RollMultiplier => (NormalizedRollPower + 1) / 2;
    public float YawMultiplier => (NormalizedRollPower + 1) / 2;
    
    // [0,2]
    public float LiftMultiplier => NormalizedLiftPower + 1;

    public bool stabilization = false;

    public AnimationCurve RotationCurve;
    public AnimationCurve LiftCurve;

    private float gravityCounter; 
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
        gravityCounter = -(Physics.gravity.y * rb.mass);

        radius = Vector3.Distance(transform.position, r1.position);
        mass = rb.mass;
        
        rb.maxAngularVelocity = 100;
    }

    public float r1Force = 0;
    public float r2Force = 0;
    public float r3Force = 0;
    public float r4Force = 0;
    
    private float hoverForce;
    
    private void FixedUpdate()
    {
        CalculateForces(ref r1Force, ref r2Force, ref r3Force, ref r4Force);
        
        rb.AddForceAtPosition(transform.up * r1Force, r1.position);
        // rb.AddForceAtPosition(transform.up * r2Force, r2.position);
        rb.AddForceAtPosition(transform.up * r3Force, r3.position);
        // rb.AddForceAtPosition(transform.up * r4Force, r4.position);
        
        // rb.AddRelativeTorque(new Vector3(0, (r1Force - r2Force + r3Force - r4Force) * maxTorque, 0));
    }

    private void CalculateForces(ref float f1, ref float f2, ref float f3, ref float f4)
    {
        f1 = f2 = f3 = f4 = 0;
        
        float angleOfAttack = AngleOfAttack;
        float angleOfAttackX = AngleOfAttackX();
        float angleOfAttackZ = AngleOfAttackZ();
        //Debug.Log("Angle of attack: " + angleOfAttack);

        HoverAssitance(ref f1, ref f2, ref f3, ref f4);
        
        // LIFT [0,2]:
        f1 *= LiftMultiplier;
        f2 *= LiftMultiplier;
        f3 *= LiftMultiplier;
        f4 *= LiftMultiplier;
        
        // Aplicamos la fuerza distribuyendola a cada rotor como suma o resta, manteniendo una suma de fuerzas igual
        float pitchDistributed = PitchForce / 2f;
        float rollDistributed = RollForce / 2f;
        float yawDistributed = YawForce / 2f;
        //float liftDistributed = LiftForce / 4f;
        
        if (angleOfAttack is > -90 and < 90)
        {
            f1 -= rollDistributed;
            f3 += rollDistributed;

            // f1 = hoverDistributed - pitchDistributed - rollDistributed + yawDistributed + LiftMultiplier;
            // f2 = hoverDistributed - pitchDistributed + rollDistributed - yawDistributed + LiftMultiplier;
            // f3 = hoverDistributed + pitchDistributed + rollDistributed + yawDistributed + LiftMultiplier;
            // f4 = hoverDistributed + pitchDistributed - rollDistributed - yawDistributed + LiftMultiplier;

            //LimitForces(ref f1, ref f2, ref f3, ref f4);
        }
        else
        {
            if (hoverStabilization)
                UpsideDownCalculations(ref f1, ref f2, ref f3, ref f4, pitchDistributed, rollDistributed, yawDistributed);
            else
            {
                f1 -= rollDistributed;
                f3 += rollDistributed;
            }
        }
    }

    private void HoverAssitance(ref float f1, ref float f2, ref float f3, ref float f4)
    {
        hoverForce = gravityCounter;
        
        if (hoverStabilization)
        {
            // Aumenta la suma de fuerzas para compensar la gravedad siempre
            // Con razón del coseno del angulo de ataque => cos = -G / F => Despejamos F
            if (!IsHorizontal())
                hoverForce = gravityCounter / Mathf.Max(Mathf.Cos(Mathf.Deg2Rad * AngleOfAttack), 0.2f);
        }
        
        // Capado a el maximo de la suma de la maxima fuerza de cada rotor
        hoverForce = Mathf.Min(hoverForce, maxRotorForce * 4);
        
        // Distribuida a cada rotor de igual manera para que la fuerza neta se aplique en el centro de masa
        float hoverDistributed = hoverForce / 4f;
        
        // 1 DIMENSION:
        f1 += hoverDistributed;
        f3 += hoverDistributed;
        f1 *= 2;
        f3 *= 2;
        
        // 2 DIMENSIONES:
        // f1 += hoverDistributed;
        // f2 += hoverDistributed;
        // f3 += hoverDistributed;
        // f4 += hoverDistributed;
    }
    
    private void UpsideDownCalculations(ref float f1, ref float f2, ref float f3, ref float f4, float pitch, float roll, float yaw)
    {
        // TODO AREEGLAR ESTO
        float midPower = maxRotorForce / 2;
        float basePower = midPower + midPower * NormalizedLiftPower;
        f1 = basePower - pitch - roll + yaw;
        f2 = basePower - pitch + roll - yaw;
        f3 = basePower + pitch + roll + yaw;
        f4 = basePower + pitch - roll - yaw;
            
            
            // Drone is upside down => Ignoramos la diferencia y aplicamos la fuerza a un solo lateral del drone
            // apagando el otro rotor
            // float midPower = maxRotorForce / 2;
            // float basePower = midPower + midPower * NormalizedLiftPower;
            //
            // float diffPitch = NormalizedPitchPower * maxRotorForce / 2;
            // float diffRoll = NormalizedRollPower * maxRotorForce / 2;
            // float diffYaw = NormalizedYawPower * maxRotorForce;
            //
            // f1 = f2 = f3 = f4 = basePower;
            //
            // // PITCH
            // if (diffPitch < 0)
            // {
            //     if (f1 - diffPitch > maxRotorForce)
            //     {
            //         f4 = maxRotorForce + diffPitch;
            //         f1 = maxRotorForce;
            //     }
            //     else
            //         f1 -= diffPitch;
            //     if (f2 - diffPitch > maxRotorForce)
            //     {
            //         f3 = maxRotorForce + diffPitch;
            //         f2 = maxRotorForce;
            //     }
            //     else
            //         f2 -= diffPitch;
            // }
            // else
            // {
            //     if (f3 + diffPitch > maxRotorForce)
            //     {
            //         f2 = maxRotorForce - diffPitch;
            //         f3 = maxRotorForce;
            //     }
            //     else
            //         f3 += diffPitch;
            //     
            //     if (f4 + diffPitch > maxRotorForce)
            //     {
            //         f1 = maxRotorForce - diffPitch;
            //         f4 = maxRotorForce;
            //     }
            //     else
            //         f4 += diffPitch;
            // }
            //
            // // ROLL
            // if (diffRoll < 0)
            // {
            //     if (f1 - diffRoll > maxRotorForce)
            //     {
            //         f2 = maxRotorForce + diffRoll;
            //         f1 = maxRotorForce;
            //     }
            //     else
            //         f1 -= diffRoll;
            //     if (f4 - diffRoll > maxRotorForce)
            //     {
            //         f3 = maxRotorForce + diffRoll;
            //         f4 = maxRotorForce;
            //     }
            //     else
            //         f4 -= diffRoll;
            // }
            // else
            // {
            //     if (f2 + diffRoll > maxRotorForce)
            //     {
            //         f1 = maxRotorForce - diffRoll;
            //         f2 = maxRotorForce;
            //     }
            //     else
            //         f2 += diffRoll;
            //
            //     if (f3 + diffRoll > maxRotorForce)
            //     {
            //         f4 = maxRotorForce - diffRoll;
            //         f3 = maxRotorForce;
            //     }
            //     else
            //         f3 += diffRoll;
            // }
            // // YAW
            // if (diffYaw < 0)
            // {
            //     if (f2 - diffYaw > maxRotorForce)
            //     {
            //         f1 = maxRotorForce + diffYaw / 2;
            //         f3 = maxRotorForce + diffYaw / 2;
            //         f2 = maxRotorForce;
            //     }
            //     else
            //         f2 -= diffYaw;
            //     if (f4 - diffYaw > maxRotorForce)
            //     {
            //         f1 = maxRotorForce + diffYaw / 2;
            //         f3 = maxRotorForce + diffYaw / 2;
            //         f4 = maxRotorForce;
            //     }
            //     else
            //         f4 -= diffYaw;
            // }
            // else
            // {
            //     if (f1 + diffYaw > maxRotorForce)
            //     {
            //         f2 = maxRotorForce - diffYaw / 2;
            //         f4 = maxRotorForce - diffYaw / 2;
            //         f1 = maxRotorForce;
            //     }
            //     else
            //         f1 += diffYaw;
            //
            //     if (f3 + diffYaw > maxRotorForce)
            //     {
            //         f2 = maxRotorForce - diffYaw / 2;
            //         f4 = maxRotorForce - diffYaw / 2;
            //         f3 = maxRotorForce;
            //     }
            //     else
            //         f3 += diffYaw;
            // }
    }

    public float AngleOfAttack => Vector3.SignedAngle(transform.up, Vector3.up, transform.forward);
    
    public float AngleOfAttackX()
    {
        // Cogemos como vector de referencia el vertical pero sin la rotacion en Z (como si cancelamos el roll)
        Vector3 eulerRot = transform.rotation.eulerAngles;
        eulerRot.z = 0;
        return Vector3.SignedAngle(transform.up, Quaternion.Euler(eulerRot) * Vector3.up, transform.forward);
    }
    
    public float AngleOfAttackZ()
    {
        // Cogemos como vector de referencia el vertical pero sin la rotacion en X (como si cancelamos el pitch)
        Vector3 eulerRot = transform.rotation.eulerAngles;
        eulerRot.x = 0;
        return Vector3.SignedAngle(Quaternion.Euler(eulerRot) * Vector3.up, transform.up, transform.right);
    }
    
    public bool IsHorizontal() => Mathf.Abs(transform.up.y - 1) < 0.0001f;

    public void Reset()
    {
        transform.rotation = Quaternion.identity;
        //transform.position = transform.position + Vector3.up * 0.5f;
        rb.angularVelocity = Vector3.zero;
    }

    private void LimitForces(ref float f1, ref float f2, ref float f3, ref float f4)
    {
        // Compensamos las fuerzas al limitar entre 0 y el MAXimo, manteniendo la SUMA CONSTANTE
        CompensateForces(ref f1, ref f3, ref f4, ref f2, 0, maxRotorForce);
        CompensateForces(ref f2, ref f4, ref f1, ref f3, 0, maxRotorForce);
        CompensateForces(ref f3, ref f1, ref f2, ref f4, 0, maxRotorForce);
        CompensateForces(ref f4, ref f2, ref f3, ref f1, 0, maxRotorForce);
    }

    private static void CompensateForces(ref float force, ref float opposite, ref float left, ref float right, float min, float max)
    {
        // Limitamos la fuerza a un minimo y un maximo, compensando las demas para mantener la SUMA = CONSTANTE
        // Si disminuimos uno, aumentamos su opuesto
        // Si eso conlleva pasarse de los limites del opuesto, aumentamos los laterales en la misma cantidad (diff / 2)
        if (force < min)
        {
            // Si al disminuir el opuesto sale negativo tendremos que disminuir los otros dos por igual
            // si no reducimos su opuesto para no modificar la suma de fuerzas
            float diff = min - force;
            if (opposite - diff < min)
            {
                if (left - diff / 2 < min || right - diff / 2 < min)
                {
                    opposite -= diff / 2;
                    left -= diff / 4;
                    right -= diff / 4;
                }
                else
                {
                    left -= diff / 2;
                    right -= diff / 2;
                }
            }
            else
                opposite -= diff;
            
            force = min;
        }
        if (force > max)
        {
            float diff = force - max;
            // if (opposite + diff > max)
            // {
            //     
            //     if (left + diff / 2 > max || right + diff / 2 > max)
            //     {
            //         opposite += diff / 2;
            //         left += diff / 4;
            //         right += diff / 4;
            //     }
            //     else
            //     {
            //         left += diff / 2;
            //         right += diff / 2;
            //     }
            // }
            // else
            //     opposite += diff;

            opposite -= diff;
            left -= diff;
            right -= diff;
            
            force = max;
        }
    }
    
    private void OnDrawGizmos()
    {
        if (Application.isEditor) return;
        
        // GRADIENT Green-Yellow-Red
        Gradient gradient = new Gradient();
        {
            GradientColorKey greenKey = new GradientColorKey(Color.green, 0);
            GradientColorKey yellowKey = new GradientColorKey(Color.yellow, 0.5f);
            GradientColorKey redKey = new GradientColorKey(Color.red, 1);
            GradientAlphaKey alphaKey = new GradientAlphaKey(1, 1);
            gradient.SetKeys(new[] {greenKey, yellowKey, redKey}, new[] {alphaKey});
        }

        Gizmos.color = gradient.Evaluate(r1Force / maxRotationalForce);
        Gizmos.DrawRay(r1.position, transform.up * (r1Force / 2));
        
        // Gizmos.color = gradient.Evaluate(r2Force / maxRotationalForce / 2);
        // Gizmos.DrawRay(r2.position, transform.up * (r2Force / 2));
        
        Gizmos.color = gradient.Evaluate(r3Force / maxRotationalForce);
        Gizmos.DrawRay(r3.position, transform.up * (r3Force / 2));
        
        // Gizmos.color = gradient.Evaluate(r4Force / maxRotationalForce / 2);
        // Gizmos.DrawRay(r4.position, transform.up * (r4Force / 2));

        // float sumForces = r1Force + r2Force + r3Force + r4Force;
        // Gizmos.color = gradient.Evaluate(sumForces / (hoverForce * 8));
        // Gizmos.DrawRay(transform.position + transform.up / 2, transform.up * (sumForces / 10));
    }
}

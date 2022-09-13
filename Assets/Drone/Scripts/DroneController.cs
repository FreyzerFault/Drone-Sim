using System;
using UnityEngine;

public class DroneController : MonoBehaviour
{
    public DroneSettingsSO droneSettings;
    public EnvironmentSettingsSO environmentSettings;
    
    public Rigidbody rb;

    public float smoothTime = 0.01f;
    public float lerpSpeed = 4;
    
    // Rotors of the drone (have to be associated to the four rotors of the drone, with the order V1,O1,V2,O2)
    public Rotor rotorCW1;
    public Rotor rotorCW2;
    public Rotor rotorCCW1;
    public Rotor rotorCCW2;
    
    [Range(-1,1)] public float yawInput = 0;
    [Range(-1,1)] public float pitchInput = 0;
    [Range(-1,1)] public float rollInput = 0;
    [Range(-1,1)] public float liftInput = 0;

    public bool noInput => yawInput == 0 && pitchInput == 0 && rollInput == 0 && liftInput == 0;

    public Quaternion rotationInput;

    public float Weight => rb.mass * Physics.gravity.magnitude;
    
    public DroneSettingsSO.Curves Curves => droneSettings.curves;

    public float HoverPower { get => Weight / droneSettings.saturationValues.maxThrottle / 4; }
    
    private void Awake()
    {
        ResetRotors();
        rb = GetComponent<Rigidbody>();
        
        rb.mass = droneSettings.parameters.mass;
        rb.drag = droneSettings.parameters.maxDragCoefficient;
        rb.angularDrag = droneSettings.parameters.angularDrag;
        rb.maxAngularVelocity = droneSettings.parameters.maxAngularSpeed;
        
        // Adjust lift curve to hover at 0.5
        // droneSettings.curves.liftCurve.MoveKey(1, new Keyframe(0, -1 + HoverPower));
        // droneSettings.curves.liftCurve.keys[0].outWeight = HoverPower;
        // droneSettings.curves.liftCurve.keys[2].inWeight = HoverPower;
    }

    private void FixedUpdate()
    {
        ResetRotors();
        
        // Propeller Forces
        HandleRotors();
        
        // If no input, tries to hover
        // if (noInput)
        //     Hover();

        // External Forces
        //ApplyDrag();
    }

    #region Rotors

    private void HandleRotors()
    {
        ApplyLift(Curves.liftCurve.Evaluate(liftInput));
        ApplyPitch(Curves.pitchCurve.Evaluate(pitchInput));
        ApplyRoll(Curves.rollCurve.Evaluate(rollInput));
        ApplyYaw(Curves.yawCurve.Evaluate(yawInput));
    }

    // Set rotors to 0.5 for hovering when drone is horizontal
    private void ResetRotors() => SetRotorsPower(HoverPower);
    
    // Set power to each rotor [0,1]
    private void SetRotorsPower(float cw1, float cw2, float ccw1, float ccw2)
    {
        rotorCW1.power = Mathf.Clamp01(cw1);
        rotorCW2.power = Mathf.Clamp01(cw2);
        rotorCCW1.power = Mathf.Clamp01(ccw1);
        rotorCCW2.power = Mathf.Clamp01(ccw2);
    }
    private void SetRotorsPower(float value) => SetRotorsPower(value, value, value, value);

    // Add power to each rotor, the result is clamped [0,1]
    private void AddRotorsPower(float cw1, float cw2, float ccw1, float ccw2)
    {
        rotorCW1.power += cw1;
        rotorCW2.power += cw2;
        rotorCCW1.power += ccw1;
        rotorCCW2.power += ccw2;
        
        rotorCW1.power = Mathf.Clamp01(rotorCW1.power);
        rotorCW2.power = Mathf.Clamp01(rotorCW2.power);
        rotorCCW1.power = Mathf.Clamp01(rotorCCW1.power);
        rotorCCW2.power = Mathf.Clamp01(rotorCCW2.power);
    }
    private void AddRotorsPower(float value) => AddRotorsPower(value, value, value, value);

    #endregion

    #region Physics

    /// <summary>
    /// Hover the Drone
    /// <para>Stabilize drone</para>
    /// </summary>
    private void Hover()
    {
        // Set power at (mg / maxThrottle) ( / 4, because we have 4 rotors)
        SetRotorsPower(HoverPower);

        // Vector3 rotation = transform.localEulerAngles;
        // Vector3 angularVelocity = transform.worldToLocalMatrix * rb.angularVelocity;
        //
        // // Fix rotation
        // if (rotation.x > 180)
        //     rotation.x -= 360;
        // if (rotation.z > 180)
        //     rotation.z -= 360;
        //
        // // Back to Hover position
        // float pitchBack = Mathf.InverseLerp(0, droneSettings.saturationValues.maxPitch, Math.Abs(rotation.x));
        // float rollBack = Mathf.InverseLerp(0, droneSettings.saturationValues.maxRoll, Math.Abs(rotation.z));
        //
        // ApplyPitch(pitchBack * (rotation.x > 0 ? 1 : -1));
        // ApplyRoll(rollBack * (rotation.z > 0 ? -1 : 1));
        //
        // // Slow angular Vel
        // float pitchVel = Mathf.InverseLerp(0, 2, Mathf.Abs(angularVelocity.x));
        // float rollVel = Mathf.InverseLerp(0, 2, Mathf.Abs(angularVelocity.z));
        //
        // ApplyPitch(pitchVel * (angularVelocity.x > 0 ? 1 : -1));
        // ApplyRoll(rollVel * (angularVelocity.z > 0 ? -1 : 1));
    }

    /// <summary>
    /// LIFT of the Drone
    /// <para>Increase all rotors equally</para>
    /// </summary>
    private void ApplyLift(float value)
    {
        AddRotorsPower(
            value,
            value,
            value,
            value
            );
    }

    /// <summary>
    /// PITCH of the Drone
    /// <para>Pitch Forward -> Increase Back and decrease Front</para>
    /// <para>Pitch Backward -> Increase Front and decrease Back</para>
    /// </summary>
    private void ApplyPitch(float value)
    {
        AddRotorsPower(
            -value, 
            value, 
            value, 
            -value
        );
        
        //AddRotorsPower(Mathf.Sin(transform.localRotation.eulerAngles.x));
    }

    /// <summary>
    /// ROLL of the Drone
    /// <para>Roll Right -> Increase Left and decrease Right</para>
    /// <para>Roll Left -> Increase Right and decrease Left</para>
    /// </summary>
    private void ApplyRoll(float value)
    {
        AddRotorsPower(
            value, 
            -value, 
            value, 
            -value
            );
    }

    /// <summary>
    /// YAW of the Drone
    /// <para>Yaw CW -> Increase CW rotors and decrease CCW</para>
    /// <para>Yaw CCW -> Increase CCW rotors and decrease CW</para>
    /// </summary>
    private void ApplyYaw(float value)
    {
        AddRotorsPower(
            -value, 
            -value, 
            value,
            value
        );
    }

    private void ApplyDrag()
    {
        float minDrag = droneSettings.parameters.minDragCoefficient;
        float maxDrag = droneSettings.parameters.maxDragCoefficient;
        
        // Drag depends on angle of attack 0-90 ([X, Z] axis rotation)
        // Angle 0 -> min drag 
        // Angle 90 -> max drag 
        Vector3 rotation = transform.rotation.eulerAngles;
        float attackAngleNormalized = 
            Mathf.InverseLerp(0, 90, Mathf.Abs(rotation.x))
            * Mathf.InverseLerp(0, 90, Mathf.Abs(rotation.z));
        float horizontalDragCoefficient = Mathf.Lerp(minDrag, maxDrag, attackAngleNormalized);

        // Vertical Drag is inversely proportional to the horizontal
        float verticalDragCoefficient = Mathf.Lerp(maxDrag, minDrag, attackAngleNormalized);

        // Square Velocity H and V components
        Vector3 velHorizontal = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        Vector3 velVertical = new Vector3(0, rb.velocity.y, 0);
        float sqrVelH = velHorizontal.sqrMagnitude;
        float sqrVelV = velVertical.sqrMagnitude;
        
        // Drag components
        float dragH = 0.5f * horizontalDragCoefficient * environmentSettings.atmosphericSettings.airDensity * sqrVelH;
        float dragV = 0.5f * verticalDragCoefficient * environmentSettings.atmosphericSettings.airDensity * sqrVelV;
        
        // Apply Forces
        rb.AddForce(-velHorizontal.normalized * dragH);
        rb.AddForce(-velVertical.normalized * dragV);

        // DEBUG
        drag = -velHorizontal.normalized * dragH + -velVertical.normalized * dragV;
    }

    #endregion

    #region Gizmos

    public Vector3 drag;
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        
        Gizmos.DrawLine(transform.position, transform.position + GetComponent<Rigidbody>().centerOfMass);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + drag);
        //Gizmos.DrawLine(transform.position, transform.position + rb.velocity);
    }

    #endregion

    public void ResetRotation()
    {
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        transform.position = transform.position + Vector3.up * 0.1f;
        rb.velocity = Vector3.zero;
    }

    private void OnDisable()
    {
        SetRotorsPower(0);
    }
}

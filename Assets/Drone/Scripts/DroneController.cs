using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneController : MonoBehaviour
{
    public DroneSettingsSO droneSettings;
    public EnvironmentSettingsSO environmentSettings;
    
    public Rigidbody rb;

    public float smoothTime = 0.01f;
    public float lerpTime = 4;
    
    // Rotors of the drone (have to be associated to the four rotors of the drone, with the order V1,O1,V2,O2)
    public Rotor rotorCW1;
    public Rotor rotorCW2;
    public Rotor rotorCCW1;
    public Rotor rotorCCW2;
    
    
    [Range(-1,1)] public float yawInput = 0;
    [Range(-1,1)] public float pitchInput = 0;
    [Range(-1,1)] public float rollInput = 0;
    [Range(-1,1)] public float liftInput = 0;

    public Quaternion rotationInput;
    

    public float Weight => rb.mass * Physics.gravity.magnitude;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }


    private float pitchVel = 0, yawVel = 0, rollVel = 0;
    private float pitch = 0, yaw = 0, roll = 0;
    private float targetYaw = 0;
    private void FixedUpdate()
    {
        // MOVING
        float targetPitch = pitchInput * droneSettings.saturationValues.maxPitch;
        targetYaw += yawInput * droneSettings.saturationValues.yawPower;
        float targetRoll = rollInput * droneSettings.saturationValues.maxRoll;

        Quaternion currentRot = transform.localRotation;

        // pitch = Mathf.SmoothDampAngle(pitch, targetPitch, ref pitchVel, smoothTime, Mathf.Infinity, Time.fixedDeltaTime);
        // yaw =  Mathf.SmoothDampAngle(yaw, targetYaw, ref yawVel, smoothTime, Mathf.Infinity, Time.fixedDeltaTime);
        // roll = Mathf.SmoothDampAngle(roll, targetRoll, ref rollVel, smoothTime, Mathf.Infinity, Time.fixedDeltaTime);

        pitch = Mathf.Lerp(pitch, targetPitch, Time.fixedDeltaTime * lerpTime);
        yaw = Mathf.Lerp(yaw, targetYaw, Time.fixedDeltaTime * lerpTime);
        roll = Mathf.Lerp(roll, targetRoll, Time.fixedDeltaTime * lerpTime);
        
        rotationInput = Quaternion.Euler(-pitch, yaw, roll);

        rb.MoveRotation(rotationInput);
        
        // LIFT
        Vector3 up = transform.up;
        up.x = up.z = 0;
        float diff = 1 - up.magnitude;
        float targetDiff = Physics.gravity.magnitude * diff;
        
        Vector3 engineForce = transform.up * ((rb.mass * Physics.gravity.magnitude + targetDiff) + liftInput * droneSettings.saturationValues.maxLift);
        rb.AddForce(engineForce, ForceMode.Force);
        
        // Set stationary power to all rotors
        // SetRotorsPower(0.5f, 0.5f, 0.5f, 0.5f);
        
        // Propeller Forces
        ApplyLift(liftInput);
        ApplyPitch(pitchInput);
        ApplyRoll(rollInput);
        ApplyYaw(yawInput);
        //Hover();

        // External Forces
        ApplyDrag();
    }


    #region Physics

    private void SetRotorsPower(float CW1, float CW2, float CCW1, float CCW2)
    {
        rotorCW1.power = CW1;
        rotorCW2.power = CW2;
        rotorCCW1.power = CCW1;
        rotorCCW2.power = CCW2;
        
        rotorCW1.power = Mathf.Clamp01(rotorCW1.power);
        rotorCW2.power = Mathf.Clamp01(rotorCW2.power);
        rotorCCW1.power = Mathf.Clamp01(rotorCCW1.power);
        rotorCCW2.power = Mathf.Clamp01(rotorCCW2.power);
    }
    
    private void AddRotorsPower(float CW1, float CW2, float CCW1, float CCW2)
    {
        rotorCW1.power += CW1;
        rotorCW2.power += CW2;
        rotorCCW1.power += CCW1;
        rotorCCW2.power += CCW2;
        
        rotorCW1.power = Mathf.Clamp01(rotorCW1.power);
        rotorCW2.power = Mathf.Clamp01(rotorCW2.power);
        rotorCCW1.power = Mathf.Clamp01(rotorCCW1.power);
        rotorCCW2.power = Mathf.Clamp01(rotorCCW2.power);
    }
    
    private void AddRotorsPower(float value)
    {
        AddRotorsPower(value, value, value, value);
    }


    /// <summary>
    /// Hover the Drone
    /// <para>Stabilize drone</para>
    /// </summary>
    private void Hover()
    {
        Vector3 rotation = transform.localEulerAngles;
        Vector3 angularVelocity = transform.worldToLocalMatrix * rb.angularVelocity;
        
        // Fix rotation
        if (rotation.x > 180)
            rotation.x -= 360;
        if (rotation.z > 180)
            rotation.z -= 360;
        
        // Back to Hover position
        float pitchBack = Mathf.InverseLerp(0, droneSettings.saturationValues.maxPitch, Math.Abs(rotation.x));
        float rollBack = Mathf.InverseLerp(0, droneSettings.saturationValues.maxRoll, Math.Abs(rotation.z));
        
        ApplyPitch(pitchBack * (rotation.x > 0 ? 1 : -1));
        ApplyRoll(rollBack * (rotation.z > 0 ? -1 : 1));
        
        // Slow angular Vel
        float pitchVel = Mathf.InverseLerp(0, 2, Mathf.Abs(angularVelocity.x));
        float rollVel = Mathf.InverseLerp(0, 2, Mathf.Abs(angularVelocity.z));
        
        ApplyPitch(pitchVel * (angularVelocity.x > 0 ? 1 : -1));
        ApplyRoll(rollVel * (angularVelocity.z > 0 ? -1 : 1));
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
        float pitchPower = value / 4;
        AddRotorsPower(
            -pitchPower, 
            pitchPower, 
            pitchPower, 
            -pitchPower
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
        float rollPower = -value / 4;
        AddRotorsPower(
            -rollPower, 
            rollPower, 
            -rollPower, 
            rollPower
            );
    }

    /// <summary>
    /// YAW of the Drone
    /// <para>Yaw CW -> Increase CW rotors and decrease CCW</para>
    /// <para>Yaw CCW -> Increase CCW rotors and decrease CW</para>
    /// </summary>
    private void ApplyYaw(float value)
    {
        float yawPower = -value / 4;
        AddRotorsPower(
            yawPower, 
            yawPower, 
            -yawPower,
            -yawPower
        );
    }
    
    private void ApplyDrag()
    {
        float minDrag = droneSettings.saturationValues.minDragCoefficient;
        float maxDrag = droneSettings.saturationValues.maxDragCoefficient;
        
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
        angleOfAttack = attackAngleNormalized;
        vDrag = verticalDragCoefficient;
        hDrag = horizontalDragCoefficient;
    }

    #endregion

    public Vector3 drag;
    public float angleOfAttack;
    
    public float vDrag;
    public float hDrag;
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        
        Gizmos.DrawLine(transform.position, transform.position + GetComponent<Rigidbody>().centerOfMass);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + drag);
        //Gizmos.DrawLine(transform.position, transform.position + rb.velocity);
        
    }
}

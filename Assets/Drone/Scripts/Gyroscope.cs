using System;
using UnityEngine;

public class Gyroscope : MonoBehaviour
{
    public Quaternion Rotation => transform.rotation;
    public Vector3 EulerRotation => normalizeAngles(transform.rotation.eulerAngles);
    
    public float maxPitch = 30;
    public float maxRoll = 30;
    
    // Error
    public float PitchError => normalizeAngle(transform.rotation.eulerAngles.x);
    public float RollError => normalizeAngle(transform.rotation.eulerAngles.z);

    // Normalize angle [-180,180]
    private float normalizeAngle(float angle)
    {
        if (angle > 180)
            angle -= 360;
        else if (angle < -180)
            angle += 360;
        
        return angle;
    }
    private Vector3 normalizeAngles(Vector3 eulerRotation)
    {
        eulerRotation.x = normalizeAngle(eulerRotation.x);
        eulerRotation.y = normalizeAngle(eulerRotation.y);
        eulerRotation.z = normalizeAngle(eulerRotation.z);
        return eulerRotation;
    }
    
    #region Debug

    public float pitchError;
    public float rollError;

    private void FixedUpdate()
    {
        pitchError = -PitchError / maxPitch;
        rollError = RollError / maxRoll;
    }

    #endregion
}

using UnityEngine;

public class Gyroscope : MonoBehaviour
{
    public Quaternion Rotation => transform.rotation;
    public Vector3 EulerRotation => Rotation.eulerAngles;
    
    // Error
    public float PitchError => EulerRotation.x;
    public float RollError => EulerRotation.z;
}

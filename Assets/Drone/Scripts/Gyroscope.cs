using UnityEngine;

namespace DroneSim
{
    public class Gyroscope : MonoBehaviour
    {
        public Quaternion Rotation => transform.rotation;
        public Vector3 EulerRotation => transform.rotation.eulerAngles.normalizeAngles();

        public float Pitch => transform.localRotation.eulerAngles.x.normalizeAngle();
        public float Roll => transform.localRotation.eulerAngles.z.normalizeAngle();
        
        
        public float AngleOfAttack => Vector3.SignedAngle(transform.up, Vector3.up, transform.forward);
        public bool IsHorizontal => Mathf.Abs(transform.up.y - 1) < 0.0001f;
    }

    public static class ExtensionMethods
    {
        
    }
}

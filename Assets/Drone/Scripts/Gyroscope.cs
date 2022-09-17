using UnityEngine;

namespace DroneSim
{
    public class Gyroscope : MonoBehaviour
    {
        public Quaternion Rotation => transform.rotation;
        public Vector3 EulerRotation => transform.rotation.eulerAngles.normalizeAngles();

        public float Pitch => transform.localRotation.eulerAngles.x.normalizeAngle();
        public float Roll => transform.localRotation.eulerAngles.z.normalizeAngle();
    }

    public static class ExtensionMethods
    {
        // Normalize angle [-180,180]
        public static float normalizeAngle(this float angle)
        {
            if (angle > 180)
                angle -= 360;
            else if (angle < -180)
                angle += 360;

            return angle;
        }

        public static Vector3 normalizeAngles(this Vector3 eulerRotation)
        {
            return new Vector3(
                eulerRotation.x.normalizeAngle(),
                eulerRotation.y.normalizeAngle(),
                eulerRotation.z.normalizeAngle()
                );
        }
    }
}

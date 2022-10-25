using Unity.VisualScripting;
using UnityEngine;

namespace DroneSim
{
    public class DroneController : MonoBehaviour
    {
        public DroneSettingsSO droneSettings;
        public EnvironmentSettingsSO environmentSettings;
        
        private Rigidbody rb;
        
        [DoNotSerialize]
        public DroneStabilizer stabilizer;

        
        public RectTransform pitchRollOutputPoint;
        public RectTransform liftYawOutputPoint;
        private float rectWidth = 0;

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
        
        // Final values
        private float yaw = 0;
        private float pitch = 0;
        private float roll = 0;
        private float lift = 0;


        public float Weight => rb.mass * Physics.gravity.magnitude;
        
        public DroneSettingsSO.Curves Curves => droneSettings.curves;

        public float HoverPower => Weight / droneSettings.saturationValues.maxThrottle / 4;
        
        
        private void Awake()
        {
            stabilizer = GetComponent<DroneStabilizer>();
            rb = GetComponent<Rigidbody>();
            
            rb.mass = droneSettings.parameters.mass;
            rb.drag = droneSettings.parameters.maxDragCoefficient;
            rb.angularDrag = droneSettings.parameters.angularDrag;
            rb.maxAngularVelocity = droneSettings.saturationValues.maxAngularSpeed;
            rb.centerOfMass = Vector3.zero;
            
            ResetRotors();

            if (pitchRollOutputPoint != null)
                rectWidth = pitchRollOutputPoint.transform.parent.GetComponent<RectTransform>().rect.width / 2;
        }

        private void Update()
        {
            if (pitchRollOutputPoint != null)
                pitchRollOutputPoint.transform.localPosition = new Vector2(roll * rectWidth, pitch * rectWidth);
            
            if (liftYawOutputPoint != null)
                liftYawOutputPoint.transform.localPosition = new Vector2(yaw * rectWidth, lift * rectWidth);
        }

        private void FixedUpdate()
        {
            ResetRotors();
            
            // ROTOR FORCES
            yaw = lift = pitch = roll = 0;
            
            if (stabilizer.enabled)
                Stabilize();
            else
            {
                lift = Curves.liftCurve.Evaluate(liftInput);
                roll = Curves.liftCurve.Evaluate(rollInput);
                pitch = Curves.liftCurve.Evaluate(pitchInput);
                yaw = Curves.liftCurve.Evaluate(yawInput);
            }
            
            HandleRotors();
            
            
            // EXTERNAL FORCES
            //ApplyDrag();
        }

        bool isReseted;
        private void Stabilize()
        {
            Vector3 targetRotation = new Vector3(
                Curves.pitchCurve.Evaluate(pitchInput) * droneSettings.saturationValues.maxPitch,
                0,
                Curves.rollCurve.Evaluate(rollInput) * droneSettings.saturationValues.maxRoll
            );
            Vector3 targetVelocity = new Vector3(
                0,
                Curves.liftCurve.Evaluate(liftInput) * droneSettings.saturationValues.maxLiftSpeed,
                0
                );
            Vector3 targetAngularVelocity = new Vector3(
                0, 
                Curves.yawCurve.Evaluate(yawInput) * droneSettings.saturationValues.maxAngularSpeed, 
                0);
            
            // Get PID results in [-1,1]
            stabilizer.StabilizeParams(targetRotation, targetVelocity, targetAngularVelocity, ref lift, ref pitch, ref roll, ref yaw);
            
            if (lift is < -1 or > 1 || pitch is < -1 or > 1 || roll is < -1 or > 1 || yaw is < -1 or > 1)
                Debug.Log("Some PID results may not be in Range [-1,1]\n" +
                          "Lift: " + lift + " Pitch: " + pitch + " Roll: " + roll + " Yaw: " + yaw);
        }

        #region Rotors

        /// <summary>
        /// Distribute lift, pitch, roll and yaw to the rotors
        /// </summary>
        private void HandleRotors()
        {
            AddRotorsPower(
                lift - pitch + roll - yaw,
                lift + pitch - roll - yaw,
                lift + pitch + roll + yaw,
                lift - pitch - roll + yaw
            );
        }

        // Set rotors to 0.5 for hovering when drone is horizontal
        private void ResetRotors() => SetRotorsPower(HoverPower);
        
        private Vector2 PowerRange => new Vector2(
            Mathf.Min(rotorCW1.power, rotorCW2.power, rotorCCW1.power, rotorCCW2.power),
            Mathf.Max(rotorCW1.power, rotorCW2.power, rotorCCW1.power, rotorCCW2.power)
            );

        /// <summary>
        /// Maintain the Rotors Power between 0 and 1 keeping the range between then
        /// </summary>
        private void MaintainPowerRange01()
        {
            Vector2 range = PowerRange;
            
            if (range.x < 0)
            {
                rotorCW1.power += -range.x;
                rotorCW2.power += -range.x;
                rotorCCW1.power += -range.x;
                rotorCCW2.power += -range.x;
            }

            if (range.y > 1)
            {
                rotorCW1.power += 1 - range.y;
                rotorCW2.power += 1 - range.y;
                rotorCCW1.power += 1 - range.y;
                rotorCCW2.power += 1 - range.y;
            }
        }

        private void ClampPower01()
        {
            rotorCW1.power = Mathf.Clamp01(rotorCW1.power);
            rotorCW2.power = Mathf.Clamp01(rotorCW2.power);
            rotorCCW1.power = Mathf.Clamp01(rotorCCW1.power);
            rotorCCW2.power = Mathf.Clamp01(rotorCCW2.power);
        }

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
            
            MaintainPowerRange01();
            //ClampPower01();
        }
        private void AddRotorsPower(float value) => AddRotorsPower(value, value, value, value);

        #endregion

        #region Physics

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
            transform.position += Vector3.up * 0.1f;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        private void OnDisable()
        {
            SetRotorsPower(0);
        }

    }
}

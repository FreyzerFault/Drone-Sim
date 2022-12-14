using UnityEngine;
using UnityEngine.UIElements;

namespace DroneSim
{
    public class DroneController : MonoBehaviour
    {
        public Camera FPVCamera;
        public Camera TPVCamera;
        public Camera StaticCamera;
        
        public enum FlightMode
        {
            Angle,      // El input es un ángulo de ataque no mayor a un límite
            Horizon,    // El input es una frecuencia angular => El dron puede girar sin límite
            Manual,     // No se usa PID, puramente fuerzas en cada rotor
            Static,     // El dron intentara mantener la posición y orientación por PID
        }
        
        public FlightMode flightMode;
        public bool hoverStabilization = true; // El dron intenta no perder altitud al moverse lateralmente
        
        public DroneSettingsSO droneSettings;
        public EnvironmentSettingsSO environmentSettings;
        
        
        [HideInInspector] public DroneStabilizer stabilizer;
        [HideInInspector] public Gyroscope gyro;
        [HideInInspector] public Accelerometer accelerometer;

        

        // Rotors of the drone (have to be associated to the four rotors of the drone, with the order V1,O1,V2,O2)
        public Rotor rotorCW1;
        public Rotor rotorCW2;
        public Rotor rotorCCW1;
        public Rotor rotorCCW2;

        [Range(-1,1)] public float yawInput = 0;
        [Range(-1,1)] public float pitchInput = 0;
        [Range(-1,1)] public float rollInput = 0;
        [Range(-1,1)] public float liftInput = 0;
        
        public bool NoInput => yawInput == 0 && pitchInput == 0 && rollInput == 0 && liftInput == 0;
        
        // Output values
        private float yaw = 0;
        private float pitch = 0;
        private float roll = 0;
        private float lift = 0;


        public float Mass { get => rb.mass; set => rb.mass = value; }
        public float Weight => rb.mass * Physics.gravity.magnitude;
        public float Radius => Vector3.Distance(transform.position, rotorCW1.transform.position);

        public DroneSettingsSO.Curves Curves => droneSettings.curves;

        public float HoverPower => Mathf.Clamp01(Weight / droneSettings.saturationValues.maxThrottle / 4);

        public GameObject debuggingUI;
        public RectTransform pitchRollOutputJoystick;
        public RectTransform liftYawOutputJoystick;
        private float rectWidth = 0;
        
        private Rigidbody rb;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            stabilizer = GetComponent<DroneStabilizer>();
            accelerometer = GetComponent<Accelerometer>();
            gyro = GetComponent<Gyroscope>();
            
            rb.mass = droneSettings.parameters.mass;
            rb.drag = droneSettings.parameters.maxDragCoefficient;
            rb.angularDrag = droneSettings.parameters.angularDrag;
            rb.maxAngularVelocity = droneSettings.saturationValues.maxAngularSpeed;
            rb.centerOfMass = Vector3.zero;
            
            ResetRotors();

            if (pitchRollOutputJoystick != null)
                rectWidth = pitchRollOutputJoystick.transform.parent.GetComponent<RectTransform>().rect.width / 2;
        }

        private void Update()
        {
            if (pitchRollOutputJoystick != null)
                pitchRollOutputJoystick.transform.localPosition = new Vector2(roll * rectWidth, pitch * rectWidth);
            
            if (liftYawOutputJoystick != null)
                liftYawOutputJoystick.transform.localPosition = new Vector2(yaw * rectWidth, lift * rectWidth);
            
            
            UpdateDebug(); 
        }

        private void FixedUpdate()
        {
            ResetRotors();
            
            // ROTOR FORCES
            yaw = lift = pitch = roll = 0;
            
            if (stabilizer.enabled)
                ProcessInputs();
            else
            {
                lift = Curves.liftCurve.Evaluate(liftInput);
                roll = Curves.liftCurve.Evaluate(rollInput);
                pitch = Curves.liftCurve.Evaluate(pitchInput);
                yaw = Curves.liftCurve.Evaluate(yawInput);
            }
            
            HandleRotors();
            
            
            // EXTERNAL FORCES
            // ApplyDrag();
        }
        
        // Procesa todos los inputs segun la configuracion del dron y genera los outputs correspondientes
        // Pitch, Roll, Yaw y Lift
        private void ProcessInputs()
        {
            // Vector3 targetRotation = new Vector3(
            //      targetAngleOfAttack.x * droneSettings.saturationValues.maxAngleOfAttack,
            //     0,
            //      targetAngleOfAttack.y * droneSettings.saturationValues.maxAngleOfAttack
            // );
            // Vector3 targetVelocity = new Vector3(
            //     0,
            //     Curves.liftCurve.Evaluate(liftInput) * droneSettings.saturationValues.maxLiftSpeed,
            //     0
            //     );
            // Vector3 targetAngularVelocity = new Vector3(
            //     0, 
            //     Curves.yawCurve.Evaluate(yawInput) * droneSettings.saturationValues.maxAngularSpeed,
            //     0);

            
            switch (flightMode)
            {
                case FlightMode.Angle:
                    // Pitch y Roll: Angle of Attack
                    // Se evalua en Curvas y se escala como porcentaje del maximo angulo de ataque configurado
                    Vector2 targetAngleOfAttack = new Vector2(Curves.pitchRollCurve.Evaluate(pitchInput), Curves.pitchRollCurve.Evaluate(rollInput));
                    targetAngleOfAttack *= droneSettings.saturationValues.maxAngleOfAttack;
            
                    // Get PID results in [-1,1] for correction
                    if (stabilizer.enabled)
                    {
                        Vector2 pitchAndRollCorrection =
                            stabilizer.StabilizeAngleOfAttack(new Vector2(targetAngleOfAttack.x, targetAngleOfAttack.y));

                        pitch = pitchAndRollCorrection.x;
                        roll = pitchAndRollCorrection.y;
                    }
                    
                    // Yaw and Lift only processed by curves
                    yaw = Curves.yawCurve.Evaluate(yawInput);
                    lift = Curves.liftCurve.Evaluate(liftInput);
                    break;
                
                case FlightMode.Horizon:
                    // El input se convierte en una frecuencia angular en X y Z (Pitch, Roll y Yaw)
                    // La unica diferencia entre Horizon y Manual es que en Horizon se usa PID para estabilizar la velocidad angular
                    Vector3 targetAngularVelocity = new Vector3(
                        Curves.pitchRollCurve.Evaluate(pitchInput),
                        Curves.yawCurve.Evaluate(yawInput),
                        Curves.pitchRollCurve.Evaluate(rollInput)
                    );
                    targetAngularVelocity *= droneSettings.saturationValues.maxAngularSpeed;

                    if (stabilizer.enabled)
                    {
                        Vector3 pitchYawRollCorrection =
                            stabilizer.StabilizeAngularVelocity(targetAngularVelocity);

                        pitch = pitchYawRollCorrection.x;
                        yaw = pitchYawRollCorrection.y;
                        roll = pitchYawRollCorrection.z;
                    }
                    
                    // Lift only processed by curves
                    lift = Curves.liftCurve.Evaluate(liftInput);   
                    break;
                
                case FlightMode.Manual:
                    // En Manual no se usa PID, solo se procesan las curvas
                    lift = Curves.liftCurve.Evaluate(liftInput);   
                    pitch = Curves.yawCurve.Evaluate(pitchInput);
                    roll = Curves.yawCurve.Evaluate(rollInput);
                    yaw = Curves.yawCurve.Evaluate(yawInput);
                    break;
                
                case FlightMode.Static:
                    Vector3 targetRotation = new Vector3(
                        Curves.pitchRollCurve.Evaluate(pitchInput) * droneSettings.saturationValues.maxAngleOfAttack,
                        0,
                        Curves.pitchRollCurve.Evaluate(rollInput) * droneSettings.saturationValues.maxAngleOfAttack
                    );
                    Vector3 targetVelocity = new Vector3(
                        0,
                        Curves.liftCurve.Evaluate(liftInput) * droneSettings.saturationValues.maxLiftSpeed,
                        0
                    );
                    Vector3 targetAngularSpeed = new Vector3(
                        0, 
                        Curves.yawCurve.Evaluate(yawInput) * droneSettings.saturationValues.maxAngularSpeed, 
                        0);
            
                    // Get PID results in [-1,1]
                    stabilizer.StabilizeParams(targetRotation, targetVelocity, targetAngularSpeed, ref lift, ref pitch, ref roll, ref yaw);
                    break;
            }

            // Lift only processed by curves
            
            // Outputs out of ranges [-1,1]
            if (lift is < -1 or > 1 || pitch is < -1 or > 1 || roll is < -1 or > 1 || yaw is < -1 or > 1)
                Debug.Log("Some PID results may not be in Range [-1,1]\n" +
                          "Lift: " + lift + " Pitch: " + pitch + " Roll: " + roll + " Yaw: " + yaw);
        }

        #region Rotors

        /// <summary>
        /// Distribuye lift, pitch, roll y yaw a cada rotor
        /// (Valores de cada uno = [-1,1])
        /// </summary>
        private void HandleRotors()
        {
            float hover;
            if (hoverStabilization && !gyro.IsHorizontal)
                // Calculamos la fuerza necesaria para mantener el dron a la misma altitud cuando esta inclinado
                // Se limita a la potencia maxima de los motores
                hover = Mathf.Clamp01(
                    Weight
                    / Mathf.Max(Mathf.Cos(Mathf.Deg2Rad * gyro.AngleOfAttack), 0.01f)
                    / droneSettings.saturationValues.maxThrottle
                    / 4);
            else
                hover = HoverPower;

            SetRotorsPower(hover);
            
            // Lift = [-1,1], pero al sumarlo el resultado no puede salir de [0,1]
            
            // POWER = 0 ----- hover ----------- 1
            //         |<-------|--------------->|
            // LIFT = -1        0                1
            
            if (lift > 0) lift = Mathf.Lerp(0, 1 - hover, lift);
            if (lift < 0) lift = -Mathf.Lerp(0, hover, -lift);

            // Reducimos el efecto de pitch, roll, yaw / 3, para que al sumarlos no salga del rango [-1,1]
            // Reducimos / 2 para que al sumarlo solo pueda salirse del limite [0,1] por un lateral
            // Suma Pitch + Roll + Yaw = [-.5, .5]
            
            // POWER           =  0 -------- 0.5 -------- 1
            //                    |<----------|---------->|
            // PITCH+ROLL+YAW = -0.5          0          0.5
            
            
            // POWER               =          0 --------------------- 1
            //                     <----------|---------->            |
            // PITCH+ROLL+YAW =  -0.5         0          0.5
            
            pitch /= 6;
            roll /= 6;
            yaw /= 6;
            
            // Distribuimos los valores de lift, pitch, roll, yaw a cada rotor
            AddRotorsPower(
                lift - pitch + roll - yaw,
                lift + pitch - roll - yaw,
                lift + pitch + roll + yaw,
                lift - pitch - roll + yaw
            );
            
            // Para evitar tener resultados fuera del rango [0,1]
            // y a la vez mantener las diferencias entre rotores (no clampeandolos)
            // desplazamos todos los valores tanta potencia como haya fuera del rango [0,1]

            // POWER               =          0 --------------------- 1
            //                     <----------|---------->            |
            // PITCH+ROLL+YAW =  -0.5         0          0.5
            // + min =              --------->|            ---------->|
            // Resultado =                    0                       1

            // POWER               =          0 --------------------- 1
            //                                |            <----------|---------->
            // PITCH+ROLL+YAW =                          -0.5         0         0.5
            // + min =                        |<----------            |<----------
            // Resultado =                    0                       1

            float max = Mathf.Max(rotorCW1.power, rotorCW2.power, rotorCCW1.power, rotorCCW2.power);
            float min = Mathf.Min(rotorCW1.power, rotorCW2.power, rotorCCW1.power, rotorCCW2.power);
            
            if (max > 1 && min < 0) 
                Debug.Log("Los valores de potencia estan fuera de los limites previstos:"
                          + "\nMax: " + max + " Min: " + min);

            if (max > 1) AddRotorsPower(1 - max);
            if (min < 0) AddRotorsPower(-min);
        }

        // Set rotors to 0.5 for hovering when drone is horizontal
        private void ResetRotors() => SetRotorsPower(0);

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
            Gizmos.DrawRay(transform.position, transform.up);
            
            //Gizmos.DrawLine(transform.position, transform.position + GetComponent<Rigidbody>().centerOfMass);

            //Gizmos.color = Color.blue;
            //Gizmos.DrawLine(transform.position, transform.position + drag);
            //Gizmos.DrawLine(transform.position, transform.position + rb.velocity);
        }

        #endregion

        #region Debugging

        private void UpdateDebug()
        {
            if (debuggingUI == null) return;
            
            Debugging debugging = debuggingUI.GetComponent<Debugging>();
            
            if (debugging == null) return;
            
            // Function Rendering
            FunctionRenderer[] functions = debuggingUI.GetComponentsInChildren<FunctionRenderer>();
            
            if (functions == null) return;
            
            if (functions.Length > 0)
            {
                functions[0].PlotPoint(Time.timeSinceLevelLoad, transform.localPosition.y);
                functions[1].PlotPoint(Time.timeSinceLevelLoad, accelerometer.Velocity.y);
                functions[2].PlotPoint(Time.timeSinceLevelLoad, accelerometer.acceleration.y);
            }
            
            debugging.RotorPower = new Vector4(rotorCW1.power, rotorCW2.power, rotorCCW1.power, rotorCCW2.power);
            
            debugging.FlightMode.text = flightMode.ToString();
            debugging.HoverStabilization.color = hoverStabilization ? Color.green : Color.red;
        }

        #endregion

        public void SwitchMode(bool next)
        {
            switch (flightMode)
            {
                case FlightMode.Angle:
                    flightMode = next ? FlightMode.Horizon : FlightMode.Static;
                    break;
                case FlightMode.Horizon:
                    flightMode = next ? FlightMode.Manual : FlightMode.Angle;
                    break;
                case FlightMode.Manual:
                    flightMode = next ? FlightMode.Static : FlightMode.Horizon;
                    break;
                case FlightMode.Static:
                    flightMode = next ? FlightMode.Angle : FlightMode.Manual;
                    break;
            }
        }
            
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

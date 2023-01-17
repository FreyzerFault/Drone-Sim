using System;
using UnityEngine;

namespace DroneSim
{
    public class DroneController : MonoBehaviour
    {
        public DroneSettingsSO droneSettings;
        
        [HideInInspector] public DronePIDController pidController;
        [HideInInspector] public Gyroscope gyro;
        [HideInInspector] public Accelerometer accelerometer;
        
        #region Rotors
        
        // Rotors of the drone (have to be associated to the four rotors of the drone, with the order V1,O1,V2,O2)
        public Rotor rotorCW1;
        public Rotor rotorCW2;
        public Rotor rotorCCW1;
        public Rotor rotorCCW2;
        
        #endregion
        

        #region Modos
        
        public enum FlightMode
        {
            Angle,      // El input es un ángulo de ataque no mayor a un límite
            Horizon,    // El input es una frecuencia angular => El dron puede girar sin límite
            Manual,     // No se usa PID, puramente fuerzas en cada rotor
            GodMode,     // El dron intentara mantener la posición y orientación por PID
        }
        
        public FlightMode flightMode;
        public bool hoverStabilization = true; // El dron intenta no perder altitud al moverse lateralmente
        

        public event Action<bool> OnFlightModeChange;
        public event Action<bool> OnHoverStabilizationToggle;

        #endregion
        
        
        #region Cameras

        public DroneCameraManager cameraManager;

        public Transform FPVposition;
        public Transform TPVposition;

        #endregion


        #region INPUTS - OUTPUTS

        [Range(-1,1)] public float yawInput = 0;
        [Range(-1,1)] public float pitchInput = 0;
        [Range(-1,1)] public float rollInput = 0;
        [Range(-1,1)] public float liftInput = 0;
        
        // No input this frame
        public bool NoInput => yawInput == 0 && pitchInput == 0 && rollInput == 0 && liftInput == 0;
        
        // Output values
        private float yaw = 0;
        private float pitch = 0;
        private float roll = 0;
        private float lift = 0;

        #endregion
        

        #region Physic Parameters
        
        // Parametros fisicos de unity
        public float Mass { get => rb.mass; set => rb.mass = value; }
        public float Weight => rb.mass * Physics.gravity.magnitude;
        public float Radius => Vector3.Distance(transform.position, rotorCW1.transform.position);

        // Potencia necesaria para mantener la altura
        public float HoverPower => Mathf.Clamp01(Weight / droneSettings.maxThrottle / 4);
        
        #endregion

        #region Environment Parameters
        private static EnvironmentSettingsSO EnvironmentSettings => LevelManager.Instance.EnvironmentSettings;
        #endregion
        
        private Rigidbody rb;
        
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            pidController = GetComponent<DronePIDController>();
            accelerometer = GetComponent<Accelerometer>();
            gyro = GetComponent<Gyroscope>();
            cameraManager = GameObject.FindWithTag("Camera Manager").GetComponent<DroneCameraManager>();
        }

        private void Start()
        {
            // Actualiza los parametros fisicos del Rigidbody
            UpdatePhysicParameters();
            
            ResetRotors();
        }

        private void UpdatePhysicParameters()
        {
            rb.mass = droneSettings.mass;
            rb.drag = droneSettings.maxDragCoefficient;
            rb.angularDrag = droneSettings.angularDrag;
            rb.maxAngularVelocity = droneSettings.maxAngularSpeed;
            rb.centerOfMass = Vector3.zero;
        }

        private void FixedUpdate()
        {
            ResetRotors();
            
            // ROTOR FORCES
            yaw = lift = pitch = roll = 0;
            
            if (pidController.enabled)
                ProcessInputs();
            else
            {
                lift = droneSettings.liftCurve.Evaluate(liftInput);
                roll = droneSettings.liftCurve.Evaluate(rollInput);
                pitch = droneSettings.liftCurve.Evaluate(pitchInput);
                yaw = droneSettings.liftCurve.Evaluate(yawInput);
            }
            
            HandleRotors();
            
            
            // EXTERNAL FORCES (Drag, Air,...)
            // ApplyDrag();
        }
        
        // Procesa todos los inputs segun la configuracion del dron y genera los outputs correspondientes:
        // Pitch, Roll, Yaw y Lift
        private void ProcessInputs()
        {
            switch (flightMode)
            {
                case FlightMode.Angle:
                    AngleMode();
                    break;
                case FlightMode.Horizon:
                    HorizonMode();
                    break;
                case FlightMode.Manual:
                    ManualMode();
                    break;
                case FlightMode.GodMode:
                    GodMode();
                    break;
            }
            
            // Check outputs out of ranges [-1,1]?
            if (lift is < -1 or > 1 || pitch is < -1 or > 1 || roll is < -1 or > 1 || yaw is < -1 or > 1)
                Debug.Log("Some PID results may not be in Range [-1,1]\n" +
                          "Lift: " + lift + " Pitch: " + pitch + " Roll: " + roll + " Yaw: " + yaw);
        }

        #region Modes

        public void SwitchMode(bool next)
        {
            switch (flightMode)
            {
                case FlightMode.Angle:
                    flightMode = next ? FlightMode.Horizon : FlightMode.Manual;
                    break;
                case FlightMode.Horizon:
                    flightMode = next ? FlightMode.Manual : FlightMode.Angle;
                    break;
                case FlightMode.Manual:
                    flightMode = next ? FlightMode.Angle : FlightMode.Horizon;
                    break;
                case FlightMode.GodMode:
                    flightMode = next ? FlightMode.Angle : FlightMode.Manual;
                    break;
            }
            OnFlightModeChange?.Invoke(next);
        }

        public void ToggleHoverStabilization()
        {
            hoverStabilization = !hoverStabilization;
            OnHoverStabilizationToggle?.Invoke(hoverStabilization);
        }


        private void AngleMode()
        {
            // Pitch y Roll: Angle of Attack
            // Se evalua en Curvas y se escala como porcentaje del maximo angulo de ataque configurado
            Vector2 targetAngleOfAttack = new Vector2(droneSettings.pitchRollCurve.Evaluate(pitchInput), droneSettings.pitchRollCurve.Evaluate(rollInput));
            targetAngleOfAttack *= droneSettings.maxAngleOfAttack;
            
            // Get PID results in [-1,1] for correction
            if (pidController.enabled)
            {
                Vector2 pitchAndRollCorrection =
                    pidController.AdjustAngleOfAttack(new Vector2(targetAngleOfAttack.x, targetAngleOfAttack.y));

                pitch = pitchAndRollCorrection.x;
                roll = pitchAndRollCorrection.y;
            }
                    
            // Yaw and Lift only processed by curves
            yaw = droneSettings.yawCurve.Evaluate(yawInput);
            lift = droneSettings.liftCurve.Evaluate(liftInput);
        }

        private void HorizonMode()
        {
            // El input se convierte en una frecuencia angular en X y Z (Pitch, Roll y Yaw)
            // La unica diferencia entre Horizon y Manual es que en Horizon se usa PID para estabilizar la velocidad angular
            Vector3 targetAngularVelocity = new Vector3(
                droneSettings.pitchRollCurve.Evaluate(pitchInput),
                droneSettings.yawCurve.Evaluate(yawInput),
                droneSettings.pitchRollCurve.Evaluate(rollInput)
            );
            targetAngularVelocity *= droneSettings.maxAngularSpeed;

            if (pidController.enabled)
            {
                Vector3 pitchYawRollCorrection =
                    pidController.AdjustAngularVelocity(targetAngularVelocity);

                pitch = pitchYawRollCorrection.x;
                yaw = pitchYawRollCorrection.y;
                roll = pitchYawRollCorrection.z;
            }
                    
            // Lift only processed by curves
            lift = droneSettings.liftCurve.Evaluate(liftInput); 
        }

        private void ManualMode()
        {
            // En Manual no se usa PID, solo se procesan las curvas
            lift = droneSettings.liftCurve.Evaluate(liftInput);   
            pitch = droneSettings.yawCurve.Evaluate(pitchInput);
            roll = droneSettings.yawCurve.Evaluate(rollInput);
            yaw = droneSettings.yawCurve.Evaluate(yawInput);
        }

        private void GodMode()
        {
            Vector3 targetRotation = new Vector3(
                droneSettings.pitchRollCurve.Evaluate(pitchInput) * droneSettings.maxAngleOfAttack,
                0,
                droneSettings.pitchRollCurve.Evaluate(rollInput) * droneSettings.maxAngleOfAttack
            );
            Vector3 targetVelocity = new Vector3(
                0,
                droneSettings.liftCurve.Evaluate(liftInput) * droneSettings.maxLiftSpeed,
                0
            );
            Vector3 targetAngularSpeed = new Vector3(
                0, 
                droneSettings.yawCurve.Evaluate(yawInput) * droneSettings.maxAngularSpeed, 
                0);
    
            // Ajusta la velocidad de LIFT y la angular de YAW
            lift = pidController.AdjustLift(targetVelocity.y);
            yaw = pidController.AdjustYaw(targetAngularSpeed.y);
            
            // Frena el dron cuando no hay inputs de pitch o roll
            if (pitchInput == 0 && rollInput == 0)
            {
                // Use Cascade PID Controller:
                // Get the PID by Horizontal Speed (0,0) -> get Angle of Attack PID [Roll,Pitch]
                Vector2 speedPID = pidController.AdjustHorizontalSpeed(Vector2.zero);

                // Overwrite the input target Angle
                targetRotation = new Vector3(
                    speedPID.y * droneSettings.maxAngleOfAttack,   // Pitch
                    targetRotation.y,                                                  // Yaw
                    speedPID.x * droneSettings.maxAngleOfAttack    // Roll
                );
            }
            
            // PITCH ROLL
            Vector3 pitchRoll = pidController.AdjustAngleOfAttack(targetRotation);
            pitch = pitchRoll.x;
            roll = pitchRoll.z;
        }
        
        #endregion
        
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
                    / droneSettings.maxThrottle
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

        #region External Physics

        private void ApplyDrag()
        {
            float minDrag = droneSettings.minDragCoefficient;
            float maxDrag = droneSettings.maxDragCoefficient;
            
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
            float dragH = 0.5f * horizontalDragCoefficient * EnvironmentSettings.airDensity * sqrVelH;
            float dragV = 0.5f * verticalDragCoefficient * EnvironmentSettings.airDensity * sqrVelV;
            
            // Apply Forces
            rb.AddForce(-velHorizontal.normalized * dragH);
            rb.AddForce(-velVertical.normalized * dragV);

            // DEBUG
            drag = -velHorizontal.normalized * dragH + -velVertical.normalized * dragV;
        }

        #endregion

        #region Gizmos

        private Vector3 drag;
        
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

        
            
        // Resetea todos los parametros de movimiento del dron para cuando se quede atascado
        public void ResetRotation()
        {
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            transform.position += Vector3.up * 0.1f;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        private void OnDisable() => SetRotorsPower(0);

        public void ActivateGodMode()
        {
            flightMode = FlightMode.GodMode;
            OnFlightModeChange?.Invoke(true);
        }
    }
}

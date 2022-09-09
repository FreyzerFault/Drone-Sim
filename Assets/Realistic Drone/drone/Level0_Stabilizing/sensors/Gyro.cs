using UnityEngine;
using System.Collections;

// GYRO SENSOR
public class Gyro : MonoBehaviour {
    
    // Rotors
    private Rotor helixV1;
    private Rotor helixV2;
    private Rotor helixO1;
    private Rotor helixO2;
    private float distanceBetweenHelixes;

    private float vDifference;
    private float oDifference;

    private float pitch = 0;
    public float getPitch() { return nPitch.getNoise(pitch); }
    private float lastPitch = 0;
    
    private float pitchVel = 0;
    public float getPitchVel() { return nPitchSpeed.getNoise(pitchVel); }
    private float lastPitchVel = 0;
    
    private float pitchAcc = 0;
    public float getPitchAcc() { return nPitchAcc.getNoise(pitchAcc); }

    private float roll = 0;
    public float getRoll() { return nRoll.getNoise(roll); }
    private float lastRoll = 0;
    
    private float rollVel = 0;
    public float getRollVel() { return nRollSpeed.getNoise(rollVel); }
    private float lastRollVel = 0;
    
    private float rollAcc = 0;
    public float getRollAcc() { return nRollAcc.getNoise(rollAcc); }

    NoiseAdder nRoll = new NoiseAdder();
    NoiseAdder nRollSpeed = new NoiseAdder();
    NoiseAdder nRollAcc = new NoiseAdder();
    NoiseAdder nPitch = new NoiseAdder();
    NoiseAdder nPitchSpeed = new NoiseAdder();
    NoiseAdder nPitchAcc = new NoiseAdder();

    public Transform body;

    Vector3 angularVelocity;
    Vector3 angularAcceleration;

    private float normalize(float n)
    {
        return Mathf.InverseLerp(0, distanceBetweenHelixes, n);
    }

    void Start()
    {
        DroneMovementController dmc = gameObject.GetComponent<DroneMovementController>();

        // Get rotors
        helixO1 = dmc.helixO1;
        helixO2 = dmc.helixO2;
        helixV1 = dmc.helixV1;
        helixV2 = dmc.helixV2;

        // Get rotors position
        Vector3 v1 = helixV1.transform.position;
        Vector3 v2 = helixV2.transform.position;
        Vector3 o1 = helixO1.transform.position;
        Vector3 o2 = helixO2.transform.position;

        // Distance rotor-rotor
        Vector3 mid_v1o1 = (v1 + o1) / 2f;
        Vector3 mid_v2o2 = (o2 + v2) / 2f;
        distanceBetweenHelixes = Vector3.Distance(mid_v1o1, mid_v2o2);
    }

    void FixedUpdate()
    {
        Vector3 v1 = helixV1.transform.position;
        Vector3 v2 = helixV2.transform.position;
        Vector3 o1 = helixO1.transform.position;
        Vector3 o2 = helixO2.transform.position;

        Vector3 mid_v1o2 = (v1 + o2) / 2f;
        Vector3 mid_v1o1 = (v1 + o1) / 2f;
        Vector3 mid_v2o2 = (o2 + v2) / 2f;
        Vector3 mid_v2o1 = (o1 + v2) / 2f;
         
        // TODO Pitch y Roll no esta bien calculado
        // PITCH
        pitch = mid_v2o2.y - mid_v1o1.y;  
        pitchVel = (pitch - lastPitch) / Time.deltaTime;        
        pitchAcc = (pitchVel - lastPitchVel) / Time.deltaTime;
        lastPitch = pitch;
        lastPitchVel = pitchVel;
        pitch = normalize(pitch);

        // ROLL
        roll = mid_v2o1.y - mid_v1o2.y;
        rollVel = (roll - lastRoll) / Time.deltaTime;
        rollAcc = (rollVel - lastRollVel) / Time.deltaTime;
        lastRoll = roll;
        lastRollVel = rollVel;
        roll = normalize(roll);

        
        // Calculate angular acceleration and velocity of the drone
        angularVelocity = DroneSettings.setZeroIflessThan(transform.GetComponent<Rigidbody>().angularVelocity, 0.00001f);
        angularAcceleration = DroneSettings.setZeroIflessThan((transform.GetComponent<Rigidbody>().angularVelocity - angularVelocity) / Time.deltaTime, 0.00001f);
    }

}

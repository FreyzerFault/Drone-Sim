using UnityEngine;

// Accelerometer Sensor
public class accelerom : MonoBehaviour {

    private Vector3 localLinearVelocity;
    public Vector3 getLocalLinearVelocity() { return noiseVel.getNoise(localLinearVelocity); }
    private Vector3 lastVelocity;

    private Vector3 localLinearAcceleration;
    public Vector3 getLinearAcceleration() { return noiseAcc.getNoise(localLinearAcceleration); }

    public Vector3 globalLinearVelocity;

    noiseAddVector3 noiseVel = new noiseAddVector3();
    noiseAddVector3 noiseAcc = new noiseAddVector3();
    
    void FixedUpdate()
    {
        // Local Linear Vel
        // Rotated by 45° to obtain the local velocity
        localLinearVelocity = DroneSettings.setZeroIflessThan(
            Quaternion.AngleAxis(45,transform.up) * transform.InverseTransformDirection(transform.GetComponent<Rigidbody>().velocity), 
            0.0001f);
        localLinearAcceleration = DroneSettings.setZeroIflessThan((localLinearVelocity - lastVelocity) / Time.deltaTime, 0.0001f);

        // Global Linear Vel
        lastVelocity = localLinearVelocity;
        globalLinearVelocity = DroneSettings.setZeroIflessThan(transform.GetComponent<Rigidbody>().velocity, 0.0001f);

    }

}

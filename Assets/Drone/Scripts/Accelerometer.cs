using UnityEngine;

public class Accelerometer : MonoBehaviour
{
    private Rigidbody rb;

    public Vector3 acceleration = Vector3.zero;
    public Vector3 angularAcceleration = Vector3.zero;
    
    private Vector3 prevVelocity = Vector3.zero;
    private Vector3 prevAngularVelocity = Vector3.zero;

    public Vector3 Velocity => transform.worldToLocalMatrix * rb.velocity;
    public Vector3 AngularVelocity => rb.angularVelocity;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        float deltaTime = Time.inFixedTimeStep ? Time.fixedDeltaTime : Time.deltaTime;
        
        // Velocity Vector
        Vector3 velocity = rb.velocity;
        acceleration = (velocity - prevVelocity) / deltaTime;
        

        Vector3 angularVelocity = rb.angularVelocity;
        angularAcceleration = (angularVelocity - prevAngularVelocity) / deltaTime;


        prevVelocity = velocity;
        prevAngularVelocity = angularVelocity;
    }
}

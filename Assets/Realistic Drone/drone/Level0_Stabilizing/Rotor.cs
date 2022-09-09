using UnityEngine;
using System.Collections;

public class Rotor : MonoBehaviour {

    // Drone Rigidbody (parent)
    private Rigidbody rBody;

    // Rotor Engine Power [0,1]
    public float power;

    // Orientation
    public bool counterclockwise = false;
    
    // Animation Active
    public bool animationActivated = false;
    
    // Up force vector of the rotor
    private LineRenderer lr;

    // Torque = Rotational Force (CW > 0, CCW < 0)
    public float Torque
    {
        get => (counterclockwise ? -1 : 1) * 
            Mathf.Lerp(
            DroneSettings.saturationValues.minTorque,
            DroneSettings.saturationValues.maxTorque,
            power);
    }
    
    void Start()
    {        
        Transform t = this.transform;
        while (t.parent != null && !t.CompareTag("Player")) 
            t = t.parent;
        
        rBody = t.GetComponent<Rigidbody>();

        lr = GetComponent<LineRenderer>();
    }

    void Update()
    {
        // Animation
        if (animationActivated)
        {
            transform.Rotate(0, 0, Torque * 700 * Time.deltaTime * (counterclockwise ? -1 : 1));
        }
        
        // Line Renderer
        lr.SetPosition(1, new Vector3(0, 0, power * 3f));
    }
    
    void FixedUpdate()
    {
        // Force upwards to drone from rotor point
        rBody.AddForceAtPosition(transform.forward * (power * Time.fixedDeltaTime * 1000), transform.position);
    }
}

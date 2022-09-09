using UnityEngine;
using UnityEngine.InputSystem;

public class Rotor : MonoBehaviour {

    // Drone (parent)
    private DroneController drone;
    private Rigidbody droneRB;

    // Rotor Engine Power [0,1]
    public float power;

    // Orientation
    public bool counterclockwise = false;
    
    // Animation Active
    public bool animationActivated = false;

    private MeshRenderer meshRenderer;
    private MeshRenderer blurMeshRenderer;
    public Texture2D[] blurTextures;

    // Torque = Rotational Force (CW > 0, CCW < 0)
    public float Throttle
    {
        get => (counterclockwise ? -1 : 1) * 
            Mathf.Lerp(
            DroneSettings.saturationValues.minThrottle,
            DroneSettings.saturationValues.maxThrottle,
            power);
    }

    public float MaxRotationSpeed => drone.droneSettings.saturationValues.MaxRotationSpeed;
    
    void Awake()
    {        
        Transform t = transform;
        if (t.parent != null && t.parent.GetComponent<DroneController>())
        {
            drone = t.parent.GetComponent<DroneController>();
            droneRB = drone.GetComponent<Rigidbody>();
        }

        meshRenderer = GetComponent<MeshRenderer>();
        blurMeshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
    }

    void Update()
    {
        // Animation
        if (animationActivated)
        {
            AnimatePropeller();
        }
    }
    
    void FixedUpdate()
    {
        // Force upwards to drone from rotor point
        ApplyUpForce();
    }

    private void AnimatePropeller()
    {
        transform.Rotate(0, 0, 
            Mathf.Lerp(0, MaxRotationSpeed, power * 2) * Time.deltaTime * (counterclockwise ? -1 : 1)
            );

        // If power < 0.5, hide propeller and show blur propeller quad
        meshRenderer.enabled = power < 0.5f;
        blurMeshRenderer.enabled = power >= 0.5f;

        // Switch between blur textures by power
        if (power > 0.5f)
        {
            if (power < 0.65f)
                blurMeshRenderer.sharedMaterial.mainTexture = blurTextures[0];
            else if (power < 0.8f)
                blurMeshRenderer.sharedMaterial.mainTexture = blurTextures[1];
            else if (power < 0.9f)
                blurMeshRenderer.sharedMaterial.mainTexture = blurTextures[2];
        }
    }

    private void ApplyUpForce()
    {
        droneRB.AddForceAtPosition(transform.forward * (power * Physics.gravity.magnitude / 2), transform.position);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.Lerp(Color.red, Color.green, power);
        
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * (power));
    }
}

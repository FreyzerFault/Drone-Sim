using UnityEngine;

public class Rotor : MonoBehaviour {

    // Drone (parent)
    public DroneController drone;

    // Rotor Engine Power [0,1]
    public float power;
    public float lastPower;
    public float smoothPower;
    public float smoothStep = 0.1f;

    // Orientation
    public bool counterclockwise = false;
    

    // Torque = Rotational Force (CW > 0, CCW < 0)
    public float Torque => Mathf.Lerp(0, MaxTorque, power) * (counterclockwise ? -1 : 1);
    
    // Throttle = upward force (Power = 0.5 => Hover => Throttle = Gravity)
    public float Throttle => power * drone.Weight / 2;

    
    public float MaxRotationSpeed => drone.droneSettings.saturationValues.maxRotationSpeed;
    public float MaxTorque => drone.droneSettings.saturationValues.maxTorque;
    
    
    // Animation Active
    public bool animationActivated = false;

    private MeshRenderer meshRenderer;
    private MeshRenderer blurMeshRenderer;
    public Texture2D[] blurTextures;
    
    private AudioSource audioSource;
    
    void Awake()
    {        
        Transform t = transform;
        if (t.parent != null && t.parent.GetComponent<DroneController>())
        {
            drone = t.parent.GetComponent<DroneController>();
            drone.rb = drone.GetComponent<Rigidbody>();
        }

        meshRenderer = GetComponent<MeshRenderer>();
        blurMeshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();

        audioSource = GetComponent<AudioSource>();

        lastPower = power;
        smoothPower = power;
    }

    void Update()
    {
        // Smooth change in power
        float powerDiff = power - lastPower;
        if (Mathf.Abs(powerDiff) > smoothStep)
            smoothPower = lastPower + (smoothStep * (powerDiff > 0 ? 1 : -1));
        else
            smoothPower = power;
        
        lastPower = smoothPower;
        
        // Animation
        if (animationActivated)
        {
            AnimatePropeller();
            SetTexture();
        }
        
        // Audio
        SetAudio();
    }
    
    void FixedUpdate()
    {
        // Force upwards to drone from rotor point
        ApplyThrottle();
        ApplyTorque();
    }


    private void AnimatePropeller()
    {
        float maxRotationSpeed = drone.droneSettings.saturationValues.maxRotationSpeed;
        transform.Rotate(0, 0, 
            Mathf.Lerp(0, maxRotationSpeed, smoothPower * 2) * Time.deltaTime * (counterclockwise ? -1 : 1)
            );
    }

    private void SetTexture()
    {
        float minRotationForBlur = 0.5f;
        // If power < 0.5, hide propeller and show blur propeller quad
        meshRenderer.enabled = smoothPower < minRotationForBlur;
        blurMeshRenderer.enabled = smoothPower >= minRotationForBlur;

        // Switch between blur textures by power
        if (smoothPower > minRotationForBlur)
        {
            blurMeshRenderer.sharedMaterial.mainTexture = smoothPower < 0.7f ? blurTextures[0] : blurTextures[1];
        }
    }
    
    private void SetAudio()
    {
        float powerSqr = smoothPower * smoothPower;
        audioSource.volume = Mathf.Lerp(0, .5f, powerSqr);
        audioSource.pitch = Mathf.Lerp(0.9f, 1.1f, powerSqr);
    }
    
    private void ApplyThrottle()
    {
        drone.rb.AddForceAtPosition(transform.forward * Throttle, transform.position);
    }
    
    private void ApplyTorque()
    {
        drone.rb.AddTorque(transform.forward * Torque);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.Lerp(Color.red, Color.green, power);
        
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * (power));
        
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * Torque);
    }
}

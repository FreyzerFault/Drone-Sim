using UnityEngine;

public class Rotor : MonoBehaviour {

    // Drone (parent)
    public DroneController drone;
    
    // Animation Active
    public bool animationActivated = false;

    private MeshRenderer meshRenderer;
    private MeshRenderer blurMeshRenderer;
    public Texture2D[] blurTextures;
    
    private AudioSource audioSource;
    
    // Orientation
    public bool counterclockwise = false;

    // Rotor Engine Power [0,1]
    public float power;
    public bool smoothAnimation = true;
    
    private float lastPower;
    private float smoothPower;
    private float smoothStep = 0.1f;

    public float Power => smoothAnimation ? smoothPower : power;

    #region Physics Parameters

    // Torque = Rotational Force applied to propeller by rotor (CW > 0, CCW < 0)
    public float Torque => Power * MaxTorque * (counterclockwise ? -1 : 1);
    
    // Throttle = upward force (Power = 0.5 => Hover => Throttle = Gravity)
    public float Throttle => Power * MaxThrottle;

    
    private float MaxRotationSpeed => drone.droneSettings.saturationValues.maxRotationSpeed;
    private float MaxTorque => drone.droneSettings.saturationValues.maxTorque;
    private float MaxThrottle => drone.droneSettings.saturationValues.maxThrottle;
    
    #endregion
    
    
    
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
        if (smoothAnimation)
        {
            // CLAMP Power [0,1]
            power = Mathf.Clamp01(power);

            // Smooth change in power
            float powerDiff = power - lastPower;
            if (Mathf.Abs(powerDiff) > smoothStep)
                smoothPower = lastPower + (smoothStep * (powerDiff > 0 ? 1 : -1));
            else
                smoothPower = power;

            lastPower = smoothPower;
        }
        else
            smoothPower = power;

        // Animation
        if (animationActivated)
        {
            AnimatePropeller(smoothPower);
            SetTexture(smoothPower);
        }
        
        // Audio
        SetAudio(smoothPower);
    }
    
    void FixedUpdate()
    {
        // CLAMP Power [0,1]
        power = Mathf.Clamp01(power);
        
        // Force upwards to drone from rotor point
        ApplyThrottle();
        ApplyTorque();
    }


    #region Animation

    /// <summary>
    /// Rotate depending on power
    /// </summary>
    /// <param name="power"></param>
    private void AnimatePropeller(float power)
    {
        float maxRotationSpeed = drone.droneSettings.saturationValues.maxRotationSpeed;
        transform.Rotate(0, 0, 
            Mathf.Lerp(0, maxRotationSpeed, power * 2) * Time.deltaTime * (counterclockwise ? -1 : 1)
        );
    }

    /// <summary>
    /// Change texture dynamicaly depending on power
    /// </summary>
    /// <param name="power"></param>
    private void SetTexture(float power)
    {
        float minRotationForBlur = 0.5f;
        // If power < 0.5, hide propeller and show blur propeller quad
        meshRenderer.enabled = power < minRotationForBlur;
        blurMeshRenderer.enabled = power >= minRotationForBlur;

        // Switch between blur textures by power
        if (power > minRotationForBlur)
        {
            blurMeshRenderer.sharedMaterial.mainTexture = power < 0.7f ? blurTextures[0] : blurTextures[1];
        }
    }
    
    /// <summary>
    /// Change audio params dynamicaly depending on power
    /// </summary>
    /// <param name="power"></param>
    private void SetAudio(float power)
    {
        float powerSqr = power * power;
        audioSource.volume = Mathf.Lerp(0, .5f, powerSqr);
        audioSource.pitch = Mathf.Lerp(0.9f, 1.1f, powerSqr);
    }

    #endregion

    #region Physics

    /// <summary>
    /// Throttle = upward force caused by air flowing down
    /// </summary>
    private void ApplyThrottle()
    {
        drone.rb.AddForceAtPosition(transform.forward * Throttle, transform.position);
    }
    
    /// <summary>
    /// Torque is based in 3º law of Newton
    /// <para>Action-Reaction principle: For every action there is an equal and opposite reaction</para>
    /// <para>Torque applied to propeller will apply a inverse torque to drone</para>
    /// </summary>
    private void ApplyTorque()
    {
        drone.rb.AddRelativeTorque(transform.forward * -Torque);
    }

    #endregion


    #region Gizmos

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.Lerp(Color.red, Color.green, power);
        
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * (power));
        
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * Torque);
    }

    #endregion
    
}

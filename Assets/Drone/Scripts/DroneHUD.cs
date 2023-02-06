using System;
using System.Globalization;
using DroneSim;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DroneHUD : MonoBehaviour
{
    private DroneController drone;
    
    #region Info Elements
    
    public Transform arrowLeft;
    public Transform arrowRight;
    public Transform arrowUp;
    public Transform arrowDown;

    public TMP_Text FlightMode;
    public TMP_Text HoverStabilization;

    #region Camera Icon

    [Serializable]
    public struct CameraSprite
    {
        public DroneCamera.CameraType Type;
        public Sprite Sprite;
    }
    public CameraSprite[] cameraSprites;
    public Image cameraIcon;

    #endregion

    public Text liftSpeedT;
    public Text horizontalSpeedXT;
    public Text horizontalSpeedZT;
    public Text yawSpeedT;

    public TMP_Text CW1text;
    public TMP_Text CW2text;
    public TMP_Text CCW1text;
    public TMP_Text CCW2text;
    public Slider CW1slider;
    public Slider CW2slider;
    public Slider CCW1slider;
    public Slider CCW2slider;

    #endregion
    
    

    #region Joysticks

    private JoystickUI joystickLeft; 
    private JoystickUI joystickRight; 

    #endregion

    #region Animations

    private static readonly int Pulse = Animator.StringToHash("pulse");
    private static readonly int Right = Animator.StringToHash("right");
    private static readonly int Left = Animator.StringToHash("left");
    private static readonly int Up = Animator.StringToHash("up");
    private static readonly int Down = Animator.StringToHash("down");

    #endregion
    
    
    private void Awake()
    {
        // JOYSTICKS
        GameObject.FindGameObjectWithTag("Joystick I").TryGetComponent(out joystickLeft);
        GameObject.FindGameObjectWithTag("Joystick D").TryGetComponent(out joystickRight);
    }

    private void Start()
    {
        drone = FindObjectOfType<DroneController>();
        
        // ANIMATIONS
        drone.OnFlightModeChange += UpdateFlightMode;
        drone.OnHoverStabilizationToggle += UpdateHoverStabilization;
        CameraManager.Instance.OnCameraSwitched += UpdateCameraIcon;

        FlightMode.text = drone.flightMode.ToString();
        UpdateHoverStabilization(drone.hoverStabilization);
        UpdateCameraIcon();
    }
    

    private void Update()
    {
        UpdateJoysticks();
        
        UpdateRotors();
    }

    private void UpdateJoysticks()
    {
        joystickLeft.SetJoystickSquared(new Vector2(drone.yawInput, drone.liftInput));
        joystickRight.SetJoystickSquared(new Vector2(drone.rollInput, drone.pitchInput));
    }
    
    // Stabilizacion de altura
    private void UpdateHoverStabilization(bool isOn)
    {
        // Animation
        if (HoverStabilization.TryGetComponent(out Animator animator))
            animator.SetTrigger(Pulse);
        if (arrowUp.TryGetComponent(out animator))
        {
            animator.SetTrigger(Up);
            animator.SetTrigger(Pulse);
        }
        
        Color darkRed = Color.red;
        Vector3 drVector = new Vector3(darkRed.r, darkRed.g, darkRed.b);
        drVector *= .7f;
        darkRed = new Color(drVector.x, drVector.y, drVector.z, 1);
        HoverStabilization.color = isOn ? Color.green : darkRed;
    }
    
    private void UpdateCameraIcon()
    {
        if (arrowDown.TryGetComponent(out Animator animator))
        {
            animator.SetTrigger(Down);
            animator.SetTrigger(Pulse);
        }
        
        cameraIcon.sprite = GetCameraIcon();
    }

    private Sprite GetCameraIcon()
    {
        foreach (CameraSprite cameraSprite in cameraSprites)
            if (cameraSprite.Type == CameraManager.Instance.ActiveCameraType)
                return cameraSprite.Sprite;
        return null;
    }
    
    private void UpdateFlightMode(bool next)
    {
        FlightMode.text = drone.flightMode.ToString();

        Animator animator;
        
        // Animation
        if (next)
        {
            if (arrowRight.TryGetComponent(out animator))
            {
                animator.SetTrigger(Right);
                animator.SetTrigger(Pulse);
            }
        }
        else
        {
            if (arrowLeft.TryGetComponent(out animator))
            {
                animator.SetTrigger(Left);
                animator.SetTrigger(Pulse);
            }
        }
            
        if (FlightMode.TryGetComponent(out animator))
            animator.SetTrigger(Pulse);
    }

    private void UpdateRotors()
    {
        Vector4 powers = RotorPower;
        
        CW1text.text = powers.x.ToString("P0", CultureInfo.InvariantCulture);
        CW2text.text = powers.y.ToString("P0", CultureInfo.InvariantCulture);
        CCW1text.text = powers.z.ToString("P0", CultureInfo.InvariantCulture);
        CCW2text.text = powers.w.ToString("P0", CultureInfo.InvariantCulture);

        CW1slider.value = powers.x;
        CW2slider.value = powers.y;
        CCW1slider.value = powers.z;
        CCW2slider.value = powers.w;

        if (CW1slider.fillRect.TryGetComponent(out Image fillImage))
            fillImage.color = Color.Lerp(Color.white, Color.cyan, powers.x);
        if (CW2slider.fillRect.TryGetComponent(out fillImage))
            fillImage.color = Color.Lerp(Color.white, Color.cyan, powers.y);
        if (CCW1slider.fillRect.TryGetComponent(out fillImage))
            fillImage.color = Color.Lerp(Color.white, Color.cyan, powers.z);
        if (CCW2slider.fillRect.TryGetComponent(out fillImage))
            fillImage.color = Color.Lerp(Color.white, Color.cyan, powers.w);
    }


    private void UpdateDebugInfo()
    {
        // Velocidades
        Vector3 velocity = Quaternion.Euler(0, -transform.rotation.eulerAngles.y.normalizeAngle(), 0) *
                           drone.accelerometer.Velocity;
        
        liftSpeedT.text = velocity.y.ToString("F4", CultureInfo.InvariantCulture);
        horizontalSpeedXT.text = velocity.x.ToString("F4", CultureInfo.InvariantCulture);
        horizontalSpeedZT.text = velocity.z.ToString("F4", CultureInfo.InvariantCulture);
        yawSpeedT.text = drone.accelerometer.AngularVelocity.y.ToString("F4", CultureInfo.InvariantCulture);
    }


    private Vector4 RotorPower =>
        new Vector4(
            drone.rotorCW1.power,
            drone.rotorCW2.power,
            drone.rotorCCW1.power,
            drone.rotorCCW2.power
        );

    private void OnDestroy()
    {
        if (drone != null)
        {
            drone.OnFlightModeChange -= UpdateFlightMode;
            drone.OnHoverStabilizationToggle -= UpdateHoverStabilization;
        }
        if (CameraManager.Instance != null)
            CameraManager.Instance.OnCameraSwitched -= UpdateCameraIcon;
    }
}
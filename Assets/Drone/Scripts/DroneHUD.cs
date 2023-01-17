using System;
using System.Globalization;
using DroneSim;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DroneHUD : MonoBehaviour
{
    public DroneController drone;
    
    #region Info Elements

    public Transform arrowLeft;
    public Transform arrowRight;
    public Transform arrowUp;
    public Transform arrowDown;

    public TMP_Text FlightMode;
    public TMP_Text HoverStabilization;
    public Image cameraIcon;

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
        drone = GameObject.FindWithTag("Drone").GetComponent<DroneController>();

        // JOYSTICKS
        GameObject[] joysticks = GameObject.FindGameObjectsWithTag("Joystick");
        if (joysticks.Length != 2) Debug.LogError("Joysticks are not found in UI");
        joystickLeft = joysticks[1].GetComponent<JoystickUI>();
        joystickRight = joysticks[0].GetComponent<JoystickUI>();
    }

    private void Start()
    {
        // ANIMATIONS
        drone.OnFlightModeChange += UpdateFlightMode;
        drone.OnHoverStabilizationToggle += UpdateHoverStabilization;
        drone.cameraManager.OnCameraSwitched += UpdateCameraIcon;

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
        HoverStabilization.GetComponent<Animator>().SetTrigger(Pulse);
        arrowUp.GetComponent<Animator>().SetTrigger(Up);
        arrowUp.GetComponent<Animator>().SetTrigger(Pulse);
        
        Color darkRed = Color.red;
        Vector3 drVector = new Vector3(darkRed.r, darkRed.g, darkRed.b);
        drVector *= .7f;
        darkRed = new Color(drVector.x, drVector.y, drVector.z, 1);
        HoverStabilization.color = isOn ? Color.green : darkRed;
    }
    
    private void UpdateCameraIcon()
    {
        arrowDown.GetComponent<Animator>().SetTrigger(Down);
        arrowDown.GetComponent<Animator>().SetTrigger(Pulse);
        
        cameraIcon.sprite = drone.cameraManager.ActiveCameraSprite;
    }

    private void UpdateFlightMode(bool next)
    {
        FlightMode.text = drone.flightMode.ToString();
        
        
        // Animation
        if (next)
        {
            arrowRight.GetComponent<Animator>().SetTrigger(Right);
            arrowRight.GetComponent<Animator>().SetTrigger(Pulse);
        }
        else
        {
            arrowLeft.GetComponent<Animator>().SetTrigger(Left);
            arrowLeft.GetComponent<Animator>().SetTrigger(Pulse);
        }
            
        FlightMode.GetComponent<Animator>().SetTrigger(Pulse);
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

        CW1slider.fillRect.GetComponent<Image>().color = Color.Lerp(Color.white, Color.cyan, powers.x);
        CW2slider.fillRect.GetComponent<Image>().color = Color.Lerp(Color.white, Color.cyan, powers.y);
        CCW1slider.fillRect.GetComponent<Image>().color = Color.Lerp(Color.white, Color.cyan, powers.z);
        CCW2slider.fillRect.GetComponent<Image>().color = Color.Lerp(Color.white, Color.cyan, powers.w);
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
}
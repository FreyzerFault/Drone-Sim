using System;
using System.Collections;
using System.Globalization;
using DroneSim;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DebuggingUI : MonoBehaviour
{
    public Transform arrowLeft;
    public Transform arrowRight;

    public TMP_Text FlightMode;
    public TMP_Text HoverStabilization;

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

    public DroneController drone;
    
    private static readonly int Pulse = Animator.StringToHash("pulse");
    private static readonly int Right = Animator.StringToHash("right");
    private static readonly int Left = Animator.StringToHash("left");

    private void Awake()
    {
        drone = GameObject.FindWithTag("Drone").GetComponent<DroneController>();

        float springArrowOffset = 30;
        drone.OnFlightModeChange += next =>
        {
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
            
            //StartCoroutine(PulseAnimation(FlightMode.transform, Vector3.zero));

            // StartCoroutine(next
            //     ? PulseAnimation(arrowRight, new Vector3(springArrowOffset, 0, 0))
            //     : PulseAnimation(arrowLeft, new Vector3(-springArrowOffset, 0, 0)));
        };
        
        drone.OnHoverStabilizationToggle += isOn =>
        {
            HoverStabilization.GetComponent<Animator>().SetTrigger(Pulse);
            UpdateHoverStabilization(isOn);
        };
        
        UpdateHoverStabilization(drone.hoverStabilization);
    }

    private void Update()
    {
        // Potencia de los rotores
        RotorPower = new Vector4(
            drone.rotorCW1.power,
            drone.rotorCW2.power,
            drone.rotorCCW1.power,
            drone.rotorCCW2.power
        );

        // Modo de vuelo
        FlightMode.text = drone.flightMode.ToString();


        // Velocidades
        Vector3 velocity = Quaternion.Euler(0, -transform.rotation.eulerAngles.y.normalizeAngle(), 0) *
                           drone.accelerometer.Velocity;
        LiftSpeed = velocity.y;
        HorizontalSpeedX = velocity.x;
        HorizontalSpeedZ = velocity.z;
        YawSpeed = drone.accelerometer.AngularVelocity.y;
    }

    // Stabilizacion de altura
    public void UpdateHoverStabilization(bool isOn)
    {
        Color darkRed = Color.red;
        Vector3 drVector = new Vector3(darkRed.r, darkRed.g, darkRed.b);
        drVector *= .7f;
        darkRed = new Color(drVector.x, drVector.y, drVector.z, 1);
        HoverStabilization.color = isOn ? Color.green : darkRed;
    }

    public float LiftSpeed
    {
        get => float.Parse(liftSpeedT.text);
        set => liftSpeedT.text = value.ToString("F4", CultureInfo.InvariantCulture);
    }

    public float HorizontalSpeedX
    {
        get => float.Parse(horizontalSpeedXT.text);
        set => horizontalSpeedXT.text = value.ToString("F4", CultureInfo.InvariantCulture);
    }

    public float HorizontalSpeedZ
    {
        get => float.Parse(horizontalSpeedZT.text);
        set => horizontalSpeedZT.text = value.ToString("F4", CultureInfo.InvariantCulture);
    }

    public float YawSpeed
    {
        get => float.Parse(yawSpeedT.text);
        set => yawSpeedT.text = value.ToString("F4", CultureInfo.InvariantCulture);
    }

    public Vector4 RotorPower
    {
        get => new Vector4(float.Parse(CW1text.text), float.Parse(CW2text.text), float.Parse(CCW1text.text),
            float.Parse(CCW2text.text));
        set
        {
            CW1text.text = value.x.ToString("P0", CultureInfo.InvariantCulture);
            CW2text.text = value.y.ToString("P0", CultureInfo.InvariantCulture);
            CCW1text.text = value.z.ToString("P0", CultureInfo.InvariantCulture);
            CCW2text.text = value.w.ToString("P0", CultureInfo.InvariantCulture);

            CW1slider.value = value.x;
            CW2slider.value = value.y;
            CCW1slider.value = value.z;
            CCW2slider.value = value.w;

            CW1slider.fillRect.GetComponent<Image>().color = Color.Lerp(Color.white, Color.cyan, value.x);
            CW2slider.fillRect.GetComponent<Image>().color = Color.Lerp(Color.white, Color.cyan, value.y);
            CCW1slider.fillRect.GetComponent<Image>().color = Color.Lerp(Color.white, Color.cyan, value.z);
            CCW2slider.fillRect.GetComponent<Image>().color = Color.Lerp(Color.white, Color.cyan, value.w);
        }
    }
}
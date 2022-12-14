using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class Debugging : MonoBehaviour
{
    public Text FlightMode;
    public Text HoverStabilization;
    
    public Text liftSpeedT;
    public Text horizontalSpeedXT;
    public Text horizontalSpeedZT;
    public Text yawSpeedT;

    public Text CW1text;
    public Text CW2text;
    public Text CCW1text;
    public Text CCW2text;
    public Slider CW1slider;
    public Slider CW2slider;
    public Slider CCW1slider;
    public Slider CCW2slider;

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
        get => new Vector4(float.Parse(CW1text.text), float.Parse(CW2text.text), float.Parse(CCW1text.text), float.Parse(CCW2text.text));
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

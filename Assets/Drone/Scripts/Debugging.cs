using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class Debugging : MonoBehaviour
{
    public Text liftSpeedT;
    public Text horizontalSpeedXT;
    public Text horizontalSpeedZT;
    public Text yawSpeedT;

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
    
    
}

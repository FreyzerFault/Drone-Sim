using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Timer))]
public class TemporalMessage : Message
{
    private Timer timer;

    public void SetTime(float timeToClose) => timer.endTime = timeToClose;
    
    protected override void Awake()
    {
        base.Awake();
        timer = GetComponent<Timer>();
    }

    public override void Open()
    {
        base.Open();
        
        timer.Reset();
        timer.Unpause();
        timer.OnEnd += Close;
    }

    public override void Close()
    {
        base.Close();
        
        timer.Pause();
        timer.Reset();
        
        timer.OnEnd -= Close;
    }
}

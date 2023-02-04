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
        timer.OnEnd += Close;

        GameManager.Instance.OnPause += Pause;
        GameManager.Instance.OnPause += Unpause;
    }
    
    public void Pause() => timer.Pause();
    public void Unpause() => timer.Unpause();
    
    public override void Open()
    {
        base.Open();
        
        timer.Reset();
        timer.Unpause();
    }

    protected override void Close()
    {
        base.Close();
        
        timer.Pause();
        timer.Reset();
    }
    
    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPause -= Pause;
            GameManager.Instance.OnPause -= Unpause;
        }
    }
}

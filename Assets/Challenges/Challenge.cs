using TMPro;
using UnityEngine;

public abstract class Challenge : MonoBehaviour
{
    public Timer timer;
    
    public bool completed = false;

    protected virtual void Awake() => timer.Pause();

    public virtual void StartChallenge() => completed = false;
    public virtual void EndChallenge() => completed = true;

    protected void StartTimer()
    {
        ResetTimer();
        UnpauseTimer();
    }
    protected void ResetTimer() => timer.Reset();
    protected void UnpauseTimer() => timer.Unpause();
    protected void PauseTimer() => timer.Pause();

    protected void ShowTimer() => timer.enabled = true;
    protected void HideTimer() => timer.enabled = false;

}

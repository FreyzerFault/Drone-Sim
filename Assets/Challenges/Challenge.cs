using TMPro;
using UnityEngine;

public abstract class Challenge : MonoBehaviour
{
    public Timer timer;
    public GameObject levelHUD;
    
    public bool completed = false;

    protected virtual void Awake()
    {
        timer = GetComponent<Timer>();
        levelHUD = GameObject.FindWithTag("Level HUD");
        
        if (timer != null)
        {
            timer.Pause();
            if (levelHUD != null)
            {
                TMP_Text[] texts = levelHUD.GetComponentsInChildren<TMP_Text>();
                timer.timerText = texts[0];
            }
        }
    }

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

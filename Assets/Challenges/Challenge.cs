using TMPro;
using UnityEngine;

public abstract class Challenge : MonoBehaviour
{
    protected Timer timer;
    protected GameObject levelHUD;
    
    public bool completed = false;

    protected virtual void Awake()
    {
        timer = GetComponent<Timer>();
        levelHUD = GameObject.FindWithTag("Level HUD");

        if (timer == null) return;
        
        timer.Pause();
        if (levelHUD != null)
        {
            TMP_Text[] texts = levelHUD.GetComponentsInChildren<TMP_Text>();
            timer.timerText = texts[0];
        }
    }

    protected virtual void StartChallenge() => completed = false;
    protected virtual void EndChallenge() => completed = true;

    #region Timer

    protected void StartTimer()
    {
        ResetTimer();
        UnpauseTimer();
    }

    private void ResetTimer() => timer.Reset();
    private void UnpauseTimer() => timer.Unpause();
    protected void PauseTimer() => timer.Pause();

    protected void ShowTimer() => timer.enabled = true;
    protected void HideTimer() => timer.enabled = false;

    #endregion
}

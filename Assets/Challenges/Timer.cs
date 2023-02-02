using System;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private bool pause = false;
    [HideInInspector] public float timeElapsed = 0;
    
    public TMP_Text timerText;

    public Color color;
    public Color pauseColor;

    // Cuando llegue al segundo endTime => OnEnd.Invoke() y se Pausa
    // Si endTime == 0 => no termina
    public float endTime = 0; // en segundos
    public event Action OnEnd;
    public bool completed = false;

    protected void Awake() => timeElapsed = 0;

    private void Start() => SuscribeEvents();

    private void OnDestroy() => UnsuscribeEvents();
    private void Update()
    {
        if (pause) return;
        
        timeElapsed += Time.deltaTime;
        
        if (timerText != null)
            timerText.text = ToString();
        
        if (endTime != 0 && endTime < timeElapsed)
        {
            Pause();
            completed = true;
            OnEnd?.Invoke();
        }
    }
    
    public void Pause()
    {
        pause = true;
        if (timerText != null)
            timerText.color = pauseColor;
    }

    public void Unpause()
    {
        if (!completed)
        {
            pause = false;
            if (timerText != null)
                timerText.color = color;   
        }
    }

    public void Reset() => timeElapsed = 0;

    private void SuscribeEvents()
    {
        GameManager.Instance.OnPause += Pause;
        GameManager.Instance.OnPause += Unpause;
    }

    private void UnsuscribeEvents()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPause -= Pause;
            GameManager.Instance.OnPause -= Unpause;
        }
    }

    public override string ToString() => string.Format("{0:00}:{1:00}:{2:00}",
        Mathf.Floor(timeElapsed / 60),
        Mathf.Floor(timeElapsed % 60),
        Mathf.Floor(timeElapsed * 1000 % 1000 / 10)
        );
}

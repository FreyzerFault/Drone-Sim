using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private bool pause = false;
    [HideInInspector] public float timeElapsed = 0;
    
    public TMP_Text timerText;

    public Color color;
    public Color pauseColor;

    protected virtual void Awake() => timeElapsed = 0;

    private void Start() => SuscribeEvents();

    private void OnDestroy() => UnsuscribeEvents();
    private void Update()
    {
        if (pause) return;
        
        timeElapsed += Time.deltaTime * 1000;
        
        if (timerText != null)
            timerText.text = ToString();
    }
    
    public void Pause()
    {
        pause = true;
        timerText.color = pauseColor;
    }

    public void Unpause()
    {
        pause = false;
        timerText.color = color;
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
        Mathf.Floor(timeElapsed / 60000),
        Mathf.Floor((timeElapsed / 1000) % 60),
        Mathf.Floor((timeElapsed % 1000) / 10)
        );
}

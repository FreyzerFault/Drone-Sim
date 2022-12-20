using UnityEngine;
using UnityEngine.UI;

public class MenuToggle : MonoBehaviour
{
    public PanelManager menuManager;
    public PanelManager settingsMenuManager;
    public Toggle toggle;


    public void Toggle()
    {
        toggle.isOn = !toggle.isOn;
    }
    
    public void OnMenuToggle(Animator anim)
    {
        if (toggle.isOn)
        {
            menuManager.OpenPanel(anim);
            menuManager.Focused = true;
            Time.timeScale = 0;
            GameManager.Instance.SwitchMode(GameMode.Pause);
        }
        else
        {
            menuManager.CloseCurrent();
            settingsMenuManager.CloseCurrent();
            Time.timeScale = 1;
            GameManager.Instance.SwitchToPreviousMode();
        }
    }
}

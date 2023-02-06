using UnityEngine.UI;

public class AudioSettingsMenu : Menu
{
    private Slider[] audioSliders;
    
    protected override void Start()
    {
        base.Start();
        
        OnOpen += LoadSettings;

        InitializeValueChangedEvents();
    }

    public void LoadSettings()
    {
        audioSliders[0].value = SettingsManager.Instance.GlobalVolume;
        audioSliders[1].value = SettingsManager.Instance.MusicVolume;
        audioSliders[2].value = SettingsManager.Instance.SFXVolume;
    }


    private void InitializeValueChangedEvents()
    {
        // OnValueChanged de Sliders => Actualiza el AudioManager
        audioSliders = GetComponentsInChildren<Slider>();
        audioSliders[0].onValueChanged.AddListener(AudioManager.SetGlobalVolume);
        audioSliders[1].onValueChanged.AddListener(AudioManager.SetMusicVolume);
        audioSliders[2].onValueChanged.AddListener(AudioManager.SetSFXVolume);
        
        // Play sound de referencia del volumen de SFX
        audioSliders[2].onValueChanged.AddListener(_ => AudioManager.Play("SFX/PauseMenu/SFXChanged"));
    }
}

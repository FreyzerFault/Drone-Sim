using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsMenu : Menu
{
    private SettingsManager Settings => SettingsManager.Instance;

    private Slider[] audioSliders;
    
    protected override void Start()
    {
        base.Start();
        
        OnOpen += LoadSettings;

        // OnValueChanged de Sliders => Actualiza el AudioManager
        audioSliders = GetComponentsInChildren<Slider>();
        audioSliders[0].onValueChanged.AddListener((volume) => { AudioManager.Instance.SetGlobalVolume(volume); });
        audioSliders[1].onValueChanged.AddListener((volume) => { AudioManager.Instance.SetMusicVolume(volume); });
        audioSliders[2].onValueChanged.AddListener(OnEffectsChanged);
    }

    private void OnEffectsChanged(float volume)
    {
        AudioManager.Instance.SetEffectsVolume(volume);
        AudioManager.Instance.Play("SFX/PauseMenu/SFXChanged");
    }

    public void LoadSettings()
    {
        audioSliders[0].value = AudioManager.Instance.GlobalVolume;
        audioSliders[1].value = AudioManager.Instance.MusicVolume;
        audioSliders[2].value = AudioManager.Instance.SFXVolume;
    }
}

using System;
using UnityEngine;

public class SettingsManager : SingletonPersistent<SettingsManager>
{
    [SerializeField] private SettingsSO settings;
    [SerializeField] private SettingsSO defaultSettings;

    public event Action onLoad;
    public event Action onSave;
    
    public float GlobalVolume
    {
        get => settings.globalVolume;
        set => settings.globalVolume = value;
    }

    public float MusicVolume
    {
        get => settings.musicVolume;
        set => settings.musicVolume = value;
    }
    public float EffectsVolume 
    {
        get => settings.effectsVolume;
        set => settings.effectsVolume = value;
    }
    public Vector2 Resolution 
    {
        get => settings.resolution;
        set => settings.resolution = value;
    }

    public bool GodMode 
    {
        get => settings.godMode;
        set => settings.godMode = value;
    }

    protected override void Awake()
    {
        base.Awake();
        Load();
        GameManager.OnQuitGame += Save;
    }

    public void ReturnToDefaultSettings()
    {
        GlobalVolume = defaultSettings.globalVolume;
        MusicVolume = defaultSettings.musicVolume;
        EffectsVolume = defaultSettings.effectsVolume;
        Resolution = defaultSettings.resolution;
        GodMode = defaultSettings.godMode;
    }
    
    public void Save()
    {
        // Audio
        PlayerPrefs.SetFloat("globalSound", GlobalVolume);
        PlayerPrefs.SetFloat("musicSound", MusicVolume);
        PlayerPrefs.SetFloat("effectsSound", EffectsVolume);

        // Graphics
        PlayerPrefs.SetFloat("ResolutionX", Resolution.x);
        PlayerPrefs.SetFloat("ResolutionY", Resolution.y);
        
        // Controls
        
        // Game
        PlayerPrefs.SetInt("GodMode", GodMode ? 1 : 0);
        
        PlayerPrefs.Save();

        onSave?.Invoke();
        Debug.Log("Settings Saved");
    }

    public void Load()
    {
        // Audio
        GlobalVolume = PlayerPrefs.GetFloat("globalSound", 50);
        MusicVolume = PlayerPrefs.GetFloat("musicSound", 50);
        EffectsVolume = PlayerPrefs.GetFloat("effectsSound", 50);

        // Graphics
        float w = PlayerPrefs.GetFloat("ResolutionX", 1920);
        float h = PlayerPrefs.GetFloat("ResolutionY", 1080);
        Resolution = new Vector2(w, h);
        
        // Controls
        
        // Game
        GodMode = PlayerPrefs.GetInt("GodMode", 0) == 1;
        
        onLoad?.Invoke();
    }
}

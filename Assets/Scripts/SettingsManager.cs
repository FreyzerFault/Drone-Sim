using System;
using UnityEngine;

public class SettingsManager : SingletonPersistent<SettingsManager>
{
    [SerializeField] private SettingsSO settings;
    [SerializeField] private SettingsSO defaultSettings;

    public event Action OnLoad;
    public event Action OnSave;
    
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
    public Vector2Int Resolution 
    {
        get => settings.resolution;
        set => settings.resolution = value;
    }

    public bool GodMode 
    {
        get => settings.godMode;
        set => settings.godMode = value;
    }

    private void Start()
    {
        Load();
        GameManager.Instance.OnQuitGame += Save;
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
        OnSave?.Invoke();
        
        // Audio
        PlayerPrefs.SetFloat("globalSound", GlobalVolume);
        PlayerPrefs.SetFloat("musicSound", MusicVolume);
        PlayerPrefs.SetFloat("effectsSound", EffectsVolume);

        // Graphics
        PlayerPrefs.SetInt("ResolutionX", Resolution.x);
        PlayerPrefs.SetInt("ResolutionY", Resolution.y);
        
        // Controls
        
        // Game
        PlayerPrefs.SetInt("GodMode", GodMode ? 1 : 0);

        PlayerPrefs.Save();

        Debug.Log("Settings Saved");
    }

    public void Load()
    {
        // Audio
        GlobalVolume = PlayerPrefs.GetFloat("globalSound", .5f);
        MusicVolume = PlayerPrefs.GetFloat("musicSound", .5f);
        EffectsVolume = PlayerPrefs.GetFloat("effectsSound", .5f);

        // Graphics
        int w = PlayerPrefs.GetInt("ResolutionX", 1920);
        int h = PlayerPrefs.GetInt("ResolutionY", 1080);
        Resolution = new Vector2Int(w, h);
        
        // Controls
        
        // Game
        GodMode = PlayerPrefs.GetInt("GodMode", 0) == 1;
        
        OnLoad?.Invoke();
        
        Debug.Log("Settings Loaded");
    }
}

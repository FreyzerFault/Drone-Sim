using System.Collections.Generic;
using System.Linq;
using DroneSim;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : SingletonPersistent<AudioManager>
{
    private SettingsManager Settings => SettingsManager.Instance;

    #region Mixer and MixerGroups

    public AudioMixer mixer;
    public AudioMixerGroup musicGroup;
    public AudioMixerGroup SFXGroup;
    public AudioMixerGroup MenuSFXGroup;

    private const string MIXER_MASTER_TAG = "MasterVolume";
    private const string MIXER_MUSIC_TAG = "MusicVolume";
    private const string MIXER_SFX_TAG = "SFXVolume";
    private const string MIXER_MENUSFX_TAG = "MenuSFXVolume";

    #endregion

    #region AudioSources especificos
    
    private AudioSource musicSource;
    private AudioSource ambientSource;
    private AudioSource auxSource;

    #endregion
    
    public Sound[] sounds;
    private Dictionary<string, Sound> soundMap = new Dictionary<string, Sound>();


    protected override void Awake()
    {
        base.Awake();

        foreach (Sound sound in sounds)
        {
            sound.SetSource(gameObject.AddComponent<AudioSource>());
            soundMap.Add(sound.name, sound);
        }
    }

    private void Start()
    {
        LoadVolumeSettings();
        
        PlayMenuMusic();
        
        Settings.OnLoad += LoadVolumeSettings;
        Settings.OnSave += SaveVolumeSettings;
        
        GameManager.Instance.OnSceneLoaded += OnSceneLoad;
        GameManager.Instance.OnSceneUnloaded += OnSceneUnload;

        GameManager.Instance.OnPause += OnPause;
        GameManager.Instance.OnUnpause += OnUnpause;
    }

    #region Scenes Loading
    
    private static void OnSceneLoad()
    {
        UpdateMixerVolumes();
        
        // MENU PRINCIPAL:
        if (SceneManager.GetActiveScene().buildIndex == 0) PlayMenuMusic();
    }

    private static void OnSceneUnload()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) StopMusic();
    }

    private static void PlayMenuMusic() => PlayMusic("Menu");

    #endregion

    #region Music
    
    public static void PlayMusic(string musicName)
    {
        Instance.musicSource = GetSound("Music/" + musicName).source;
        Instance.musicSource.Play();
    }
    public static void PlayMusic() => Instance.musicSource.Play();
    public static void StopMusic() => Instance.musicSource.Stop();

    #endregion

    #region Ambient

    public static void PlayAmbient(string soundName)
    {
        Instance.ambientSource = GetSound("Ambient/" + soundName).source; 
        Instance.ambientSource.Play();
    }
    public static void PlayAmbient() => Instance.ambientSource.Play();
    public static void StopAmbient() => Instance.ambientSource.Stop();

    #endregion

    #region Play/Stop/Pause Sound
    
    public static void Play(string soundName)
    {
        Sound sound = GetSound(soundName);
        if (sound != null)
            sound.Play();
    }

    public static void Stop(string soundName)
    {
        Sound sound = GetSound(soundName);
        if (sound != null && sound.IsPlaying)
            sound.Stop();
    }

    public static void Pause(string soundName)
    {
        Sound sound = GetSound(soundName);
        if (sound != null)
            sound.Pause();
    }
    public static void UnPause(string soundName)
    {
        Sound sound = GetSound(soundName);
        if (sound != null)
            sound.Unpause();
    }

    public static AudioClip GetAudioClip(string soundName)
    {
        Sound sound = GetSound(soundName);
        if (sound != null)
            return sound.clip;
        return null;
    }

    private static Sound GetSound(string soundName)
    {
        if (Instance.soundMap.TryGetValue(soundName, out Sound sound))
            return sound;
        else
            Debug.LogError("Sound " + soundName + " not found in AudioManager!");
        return null;
    }
    
    #endregion
    
    #region Pause

    private static List<AudioSource> _pausedSources = new List<AudioSource>();
    
    private static void OnPause()
    {
        // Reduce el volumen de la musica y mutea los SFX que no sean del menu
        Instance.mixer.SetFloat(MIXER_MUSIC_TAG, LinearToLogAudio(Instance.GlobalVolume / 10));

        foreach (Sound sound in Instance.soundMap.Values)
        {
            AudioSource source = sound.source;
            if (source.isPlaying && source != Instance.musicSource)
            {
                _pausedSources.Add(source);
                source.Pause();
            }
        }

        AudioSource[] spatialSources = FindSpatialAudioSources();
        foreach (AudioSource source in spatialSources)
        {
            if (source.isPlaying)
            {
                _pausedSources.Add(source);
                source.Pause();
            }
        }
        
    }

    private static void OnUnpause()
    {
        if (_pausedSources.Count != 0)
        {
            foreach (AudioSource source in _pausedSources)
                if (source != null)
                    source.UnPause();

            _pausedSources.Clear();
        }
        
        UpdateMixerVolumes();
    }

    #endregion

    #region Settings

    private void LoadVolumeSettings()
    {
        GlobalVolume = Settings.GlobalVolume;
        MusicVolume = Settings.MusicVolume;
        SfxVolume = Settings.EffectsVolume;

        UpdateMixerVolumes();
    }

    private void SaveVolumeSettings()
    {
        Settings.GlobalVolume = GlobalVolume;
        Settings.MusicVolume = MusicVolume;
        Settings.EffectsVolume = SfxVolume;
    }

    #endregion

    #region Volume
    
    public float GlobalVolume { get; private set; }
    public float MusicVolume { get; private set; }
    public float SfxVolume { get; private set; }
    
    public static void SetGlobalVolume(float volume)
    {
        Instance.GlobalVolume = volume;
        Instance.mixer.SetFloat(MIXER_MASTER_TAG, LinearToLogAudio(volume));
    }

    public static void SetMusicVolume(float volume)
    {
        Instance.MusicVolume = volume;
        Instance.mixer.SetFloat(MIXER_MUSIC_TAG, LinearToLogAudio(volume));
    }

    public static void SetSFXVolume(float volume)
    {
        Instance.SfxVolume = volume;
        Instance.mixer.SetFloat(MIXER_SFX_TAG, LinearToLogAudio(volume));
        Instance.mixer.SetFloat(MIXER_MENUSFX_TAG, LinearToLogAudio(volume));
    }
    
    private static void UpdateMixerVolumes()
    {
        Instance.mixer.SetFloat(MIXER_MASTER_TAG, LinearToLogAudio(Instance.GlobalVolume));
        Instance.mixer.SetFloat(MIXER_MUSIC_TAG, LinearToLogAudio(Instance.MusicVolume));
        Instance.mixer.SetFloat(MIXER_SFX_TAG, LinearToLogAudio(Instance.SfxVolume));
        Instance.mixer.SetFloat(MIXER_MENUSFX_TAG, LinearToLogAudio(Instance.SfxVolume));
    }

    #endregion

    #region Spatial Sounds
    
    private static AudioSource[] FindSpatialAudioSources()
    {
        List<AudioSource> spatialSources = FindObjectsOfType<AudioSource>().ToList();

        // Quita todos los sonidos que no sean espaciales (Musica, ambiente, y sonidos del AudioManager)
        spatialSources.Remove(Instance.musicSource);
        spatialSources.Remove(Instance.ambientSource);
        spatialSources.Remove(Instance.auxSource);

        foreach (Sound sound in Instance.sounds) spatialSources.Remove(sound.source);

        return spatialSources.ToArray();
    }

    #endregion

    private static float LinearToLogAudio(float t) => Mathf.Log10(Mathf.Max(t, .00001f)) * 20;
}
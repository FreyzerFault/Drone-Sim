using System.Collections.Generic;
using System.Linq;
using DroneSim;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : SingletonPersistent<AudioManager>
{
    private SettingsManager Settings => SettingsManager.Instance;

    public AudioMixer mixer;
    public AudioMixerGroup musicGroup;
    public AudioMixerGroup SFXGroup;
    public AudioMixerGroup MenuSFXGroup;

    private const string MIXER_MASTER_TAG = "MasterVolume";
    private const string MIXER_MUSIC_TAG = "MusicVolume";
    private const string MIXER_SFX_TAG = "SFXVolume";
    private const string MIXER_MENUSFX_TAG = "MenuSFXVolume";
    
    private AudioSource musicSource;
    private AudioSource ambientSource;
    private AudioSource auxSource;
    
    public Sound[] sounds;
    private Dictionary<string, Sound> soundMap = new Dictionary<string, Sound>();


    public float GlobalVolume { get; private set; }
    public float MusicVolume { get; private set; }
    public float SFXVolume { get; private set; }


    protected override void Awake()
    {
        base.Awake();

        foreach (Sound sound in sounds)
        {
            sound.SetSource(gameObject.AddComponent<AudioSource>());
            soundMap.Add(sound.name, sound);
        }
    }
    
    private void OnDestroy() => SceneManager.sceneLoaded -= OnSceneLoad;

    private void Start()
    {
        Settings.OnLoad += LoadVolumeSettings;
        Settings.OnSave += SaveVolumeSettings;
        
        SceneManager.sceneLoaded += OnSceneLoad;
        SceneManager.sceneUnloaded += OnSceneUnload;

        GameManager.Instance.OnPause += OnPause;
        GameManager.Instance.OnUnpause += OnUnpause;

        LoadVolumeSettings();
        
        PlayMenuMusic();
    }

    #region Scenes Loading
    
    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        UpdateMixerVolumes();
        
        // MENU PRINCIPAL:
        if (scene.buildIndex == 0) PlayMenuMusic();
    }

    private void OnSceneUnload(Scene scene)
    {
        if (scene.buildIndex == 0) StopMusic();
    }

    private void PlayMenuMusic() => PlayMusic("Menu");

    #endregion

    #region Music
    
    public void PlayMusic(string musicName)
    {
        musicSource = soundMap["Music/" + musicName].source; 
        musicSource.Play();
    }
    public void PlayMusic() => musicSource.Play();
    public void StopMusic() => musicSource.Stop();

    #endregion

    #region Ambient

    public void PlayAmbient(string soundName)
    {
        ambientSource = soundMap["Ambient/" + soundName].source; 
        ambientSource.Play();
    }
    public void PlayAmbient() => ambientSource.Play();
    public void StopAmbient() => ambientSource.Stop();

    #endregion

    #region Play/Stop/Pause Sound
    
    public void Play(string soundName)
    {
        Sound sound = GetSound(soundName);
        if (sound != null)
            sound.Play();
    }

    public void Stop(string soundName)
    {
        Sound sound = GetSound(soundName);
        if (sound != null && sound.IsPlaying)
            sound.Stop();
    }

    public void Pause(string soundName)
    {
        Sound sound = GetSound(soundName);
        if (sound != null)
            sound.Pause();
    }
    public void UnPause(string soundName)
    {
        Sound sound = GetSound(soundName);
        if (sound != null)
            sound.Unpause();
    }

    public AudioClip GetAudioClip(string soundName)
    {
        Sound sound = GetSound(soundName);
        if (sound != null)
            return sound.clip;
        return null;
    }

    private Sound GetSound(string soundName)
    {
        if (soundMap.TryGetValue(soundName, out Sound sound))
            return sound;
        else
            Debug.LogError("Sound " + soundName + " not found in AudioManager!");
        return null;
    }
    
    #endregion
    
    #region Pause

    private List<AudioSource> pausedSources = new List<AudioSource>();
    
    private void OnPause()
    {
        // Reduce el volumen de la musica y mutea los SFX que no sean del menu
        mixer.SetFloat(MIXER_MUSIC_TAG, LinearToLogAudio(GlobalVolume / 10));

        foreach (Sound sound in soundMap.Values)
        {
            AudioSource source = sound.source;
            if (source.isPlaying && source != musicSource)
            {
                pausedSources.Add(source);
                source.Pause();
            }
        }

        AudioSource[] spatialSources = FindSpatialAudioSources();
        foreach (AudioSource source in spatialSources)
        {
            if (source.isPlaying)
            {
                pausedSources.Add(source);
                source.Pause();
            }
        }
        
    }

    private void OnUnpause()
    {
        if (pausedSources.Count != 0)
        {
            foreach (AudioSource source in pausedSources)
                if (source != null)
                    source.UnPause();

            pausedSources.Clear();
        }
        
        UpdateMixerVolumes();
    }

    #endregion

    #region Settings

    private void LoadVolumeSettings()
    {
        GlobalVolume = Settings.GlobalVolume;
        MusicVolume = Settings.MusicVolume;
        SFXVolume = Settings.EffectsVolume;

        UpdateMixerVolumes();
    }

    private void SaveVolumeSettings()
    {
        Settings.GlobalVolume = GlobalVolume;
        Settings.MusicVolume = MusicVolume;
        Settings.EffectsVolume = SFXVolume;
    }

    #endregion

    #region Volume

    private void UpdateMixerVolumes()
    {
        mixer.SetFloat(MIXER_MASTER_TAG, LinearToLogAudio(GlobalVolume));
        mixer.SetFloat(MIXER_MUSIC_TAG, LinearToLogAudio(MusicVolume));
        mixer.SetFloat(MIXER_SFX_TAG, LinearToLogAudio(SFXVolume));
        mixer.SetFloat(MIXER_MENUSFX_TAG, LinearToLogAudio(SFXVolume));
    }

    public void SetGlobalVolume(float volume)
    {
        GlobalVolume = volume;
        mixer.SetFloat(MIXER_MASTER_TAG, LinearToLogAudio(volume));
    }

    public void SetMusicVolume(float volume)
    {
        MusicVolume = volume;
        mixer.SetFloat(MIXER_MUSIC_TAG, LinearToLogAudio(volume));
    }

    public void SetEffectsVolume(float volume)
    {
        SFXVolume = volume;
        mixer.SetFloat(MIXER_SFX_TAG, LinearToLogAudio(volume));
        mixer.SetFloat(MIXER_MENUSFX_TAG, LinearToLogAudio(volume));
    }

    #endregion

    #region Spatial Sounds
    
    private AudioSource[] FindSpatialAudioSources()
    {
        List<AudioSource> spatialSources = FindObjectsOfType<AudioSource>().ToList();

        // Quita todos los sonidos que no sean espaciales (Musica, ambiente, y sonidos del AudioManager)
        spatialSources.Remove(musicSource);
        spatialSources.Remove(ambientSource);
        spatialSources.Remove(auxSource);

        foreach (Sound sound in sounds) spatialSources.Remove(sound.source);

        return spatialSources.ToArray();
    }

    #endregion

    private static float LinearToLogAudio(float t) => Mathf.Log10(Mathf.Max(t, .00001f)) * 20;
}
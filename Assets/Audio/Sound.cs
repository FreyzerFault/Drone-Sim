using System;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public class Sound
{
    [HideInInspector] public AudioSource source;
    
    public string name;

    public AudioClip clip;
    public AudioMixerGroup mixerGroup;

    [Range(0, 1)]public float volume = .5f;
    [Range(-3, 3)]public float pitch = 1;
    public bool loop = false;

    public bool IsPlaying => source.isPlaying;

    public void SetSource(AudioSource newSource)
    {
        source = newSource;
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.loop = loop;
        source.outputAudioMixerGroup = mixerGroup;
        source.playOnAwake = false;
    }
    
    
    public void Play() => source.Play();
    public void Stop() => source.Stop();
    public void Pause() => source.Pause();
    public void Unpause() => source.UnPause();
}

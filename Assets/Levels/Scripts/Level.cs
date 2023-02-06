using System;
using DroneSim;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "Level", order = 2)]
public class Level : ScriptableObject
{
    [HideInInspector] public int ID;
    public string sceneName;
    public string previewName;
    public Sprite previewImage;

    public bool completed = false;
    
    // Indice de la escena
    [HideInInspector] public int buildIndex;

    // MUSICA y SONIDO AMBIENTE
    public string music;
    public string ambient;

    // Configuracion física del entorno (gravedad, viento, atmosfera)
    public EnvironmentSettingsSO EnvironmentSettings;


    public event Action OnLoad;
    public event Action OnUnload;

    public void Load()
    {
        EnvironmentSettings.ApplySettings();

        AudioManager.PlayMusic(music);
        AudioManager.PlayAmbient(ambient);
        
        OnLoad?.Invoke();
    }

    public void Unload()
    {
        AudioManager.StopMusic();
        AudioManager.StopAmbient();
        
        OnUnload?.Invoke();
    }
}

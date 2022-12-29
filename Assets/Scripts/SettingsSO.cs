using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Settings File", menuName = "Settings/GlobalSettings")]
public class SettingsSO : ScriptableObject
{
    [Header("Audio")] public float globalVolume;
    public float musicVolume;
    public float effectsVolume;


    [Header("Graficos")] public Vector2 resolution;

    [Header("Controles")] [Header("Game")] public bool godMode;

}

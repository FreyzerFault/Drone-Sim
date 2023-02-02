using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Settings File", menuName = "Settings/GlobalSettings")]
public class SettingsSO : ScriptableObject
{
    [Header("Audio")]
    [Range(0, 1)] public float globalVolume;
    [Range(0, 1)] public float musicVolume;
    [Range(0, 1)] public float effectsVolume;


    [Header("Graficos")] public Vector2Int resolution;

    [Header("Controles")] [Header("Game")] public bool godMode;
}
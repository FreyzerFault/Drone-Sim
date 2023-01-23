using System;
using DroneSim;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "Level", order = 2)]
public class Level : ScriptableObject
{
    [HideInInspector] public int ID;
    public string name;
    public Sprite previewImage;

    [HideInInspector] public int buildIndex;

    public EnvironmentSettingsSO EnvironmentSettings;

    public bool Completed;

    public event Action OnLoad;
    public event Action OnUnload;

    public void Load()
    {
        EnvironmentSettings.ApplySettings();
        OnLoad?.Invoke();
    }

    public void Unload() => OnUnload?.Invoke();
}

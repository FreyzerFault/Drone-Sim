using System;
using System.Collections.Generic;
using DroneSim;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : SingletonPersistent<LevelManager>
{
    [Serializable]
    public struct Level
    {
        [HideInInspector] public int ID;
        
        public string Name;
        public EnvironmentSettingsSO EnvironmentSettings;
        
        public bool Completed;

        public event Action OnLoad;
        public event Action OnQuit;

        public void Load() => OnLoad.Invoke();
        public void Quit() => OnQuit.Invoke();
    }

    [SerializeField] private Level[] levels;
    private Dictionary<string, Level> levelMap;

    public Level CurrentLevel;

    protected override void Awake()
    {
        base.Awake();
        
        // Crea el mapa de Niveles para consultarlos por nombre
        levelMap = new Dictionary<string, Level>();
        for (int i = 0; i < levels.Length; i++)
        {
            Level level = levels[i];
            level.ID = i;
            
            levelMap.Add(level.Name, level);
        }
        
        CurrentLevel = levels[0];
    }

    public void PlayLevel(int levelID)
    {
        CurrentLevel.Quit();

        CurrentLevel = levels[levelID];
        
        SceneManager.LoadScene(CurrentLevel.Name);
        
        CurrentLevel.Load();
    }

    public void QuitLevel()
    {
        CurrentLevel.Quit();
        SceneManager.LoadScene(0); // 0 deberia ser el menu principal
    }


    public void PlayLevel(string levelName) => PlayLevel(levelMap[levelName].ID);


    public Level GetLevel(string levelName) => levelMap[levelName];
    
    private void UpdateGravity(float newGravity) => Physics.gravity = new Vector3(0, -newGravity, 0); 
}

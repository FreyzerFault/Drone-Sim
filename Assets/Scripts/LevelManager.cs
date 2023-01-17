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
            Debug.Log("Level " + name + " Loaded");
            OnLoad?.Invoke();
        }

        public void Unload()
        {
            Debug.Log("Level " + name + " Unloaded");
            OnUnload?.Invoke();
        }
    }
    
    public string levelsPath = "Levels/";

    public Level[] levels;
    private Dictionary<string, Level> levelMap;

    [HideInInspector] public Level currentLevel;

    public EnvironmentSettingsSO EnvironmentSettings => currentLevel.EnvironmentSettings;
    private bool InMainMenu => SceneManager.GetActiveScene().buildIndex == 0;

    protected override void Awake()
    {
        base.Awake();
        
        // Crea el mapa de Niveles para consultarlos por nombre
        levelMap = new Dictionary<string, Level>();
        for (int i = 0; i < levels.Length; i++)
        {
            levels[i].ID = i;
            
            levels[i].buildIndex = SceneUtility.GetBuildIndexByScenePath(levelsPath + levels[i].name + ".unity");

            levelMap.Add(levels[i].name, levels[i]);
        }
        
        // Carga cual fue el ultimo nivel seleccionado
        LoadSelectedLevelPref();

        // Cada vez que carga una escena guarda el nivel
        SceneManager.sceneLoaded += (scene, mode) => SaveSelectedLevelPref();
        
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // LEVEL
        if (levelMap.ContainsKey(scene.name))
            levelMap[scene.name].Load();
        
        // Main Menu
        else if (scene.name == "Main Menu")
            Debug.Log("Main Menu loaded");
        
        // Not Found!!
        else
            Debug.Log("No hay ningun nivel guardado para esta escena: " + scene.name);
    }

    private void OnSceneUnloaded(Scene scene)
    {
        if (levelMap.ContainsKey(scene.name))
            levelMap[scene.name].Unload();
    }


    public void LoadSelectedLevel() => SceneManager.LoadScene(currentLevel.buildIndex);
    public void LoadLevel(int levelID)
    {
        currentLevel = levels[levelID];
        LoadSelectedLevel();
    }
    
    public void LoadLevel(string levelName) => LoadLevel(levelMap[levelName].ID);

    public Level GetLevel(string levelName) => levelMap[levelName];

    // Carga el menu, la escena 0 deberia ser el menu principal
    public void QuitLevel() => SceneManager.LoadScene(0);

    public static readonly string SelectedLevelSavePath = "selected level";
    
    public void LoadSelectedLevelPref() => currentLevel = levels[PlayerPrefs.GetInt(SelectedLevelSavePath, 0)];
    public void SaveSelectedLevelPref() => PlayerPrefs.SetInt(SelectedLevelSavePath, currentLevel.ID);
}

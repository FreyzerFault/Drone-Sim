using System;
using System.Collections.Generic;
using DroneSim;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : SingletonPersistent<LevelManager>
{
    public string levelsPath = "Assets/Levels/";

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
    }

    private void Start()
    {
        GameManager.Instance.OnSceneLoaded += OnSceneLoaded;
        GameManager.Instance.OnSceneUnloaded += OnSceneUnloaded;
    }

    private void OnSceneLoaded()
    {
        // Cada vez que carga una escena guarda el nivel
        SaveSelectedLevelPref();

        Scene scene = SceneManager.GetActiveScene();
            
        // LEVEL
        if (levelMap.ContainsKey(scene.name))
            levelMap[scene.name].Load();
        
        // Not Found!!
        else if (scene.name != "Main Menu")
            Debug.Log("No hay ningun nivel guardado para esta escena: " + scene.name);
    }

    private void OnSceneUnloaded()
    {
        Scene scene = SceneManager.GetActiveScene();
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
    public static void ResetLevel() => GameManager.ResetScene();
    public static void QuitLevel() => SceneManager.LoadScene(0);

    public static readonly string SelectedLevelSavePath = "selected level";
    
    public void LoadSelectedLevelPref() => currentLevel = levels[PlayerPrefs.GetInt(SelectedLevelSavePath, 0)];
    public void SaveSelectedLevelPref() => PlayerPrefs.SetInt(SelectedLevelSavePath, currentLevel.ID);
}

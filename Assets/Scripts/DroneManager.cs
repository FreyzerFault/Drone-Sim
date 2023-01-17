using DroneSim;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DroneManager : SingletonPersistent<DroneManager>
{
    public DroneSettingsSO[] drones;
    public int currentDroneIndex;
    public DroneSettingsSO CurrentDrone => drones[currentDroneIndex];

    public GameObject[] prefabs;
    public GameObject cameraManagerPrefab;

    public GameObject SpawnPoint => GameObject.FindWithTag("Initial Point");

    protected override void Awake()
    {
        base.Awake();
        
        // Carga cual fue el ultimo nivel seleccionado
        LoadSelectedDronePref();

        // Cada vez que cargue un nivel destruye el anterior dron y spawnea uno nuevo
        SceneManager.sceneLoaded += (scene, mode) => DestroyDrone();
        SceneManager.sceneLoaded += (scene, mode) => LoadDrone();
    }
    
    public void LoadDrone(Transform spawnPoint)
    {
        GameObject drone = Instantiate(CurrentDrone.prefab, spawnPoint);
        drone.GetComponent<DroneController>().droneSettings = CurrentDrone;
        GameObject cameraManager = Instantiate(cameraManagerPrefab, spawnPoint);
    }

    public void LoadDrone() => LoadDrone(SpawnPoint.transform);

    public void DestroyDrone() => Destroy(FindObjectOfType<DroneController>());

    public static readonly string SelectedDroneSavePath = "selected drone";
    private void LoadSelectedDronePref() => currentDroneIndex = PlayerPrefs.GetInt(SelectedDroneSavePath, 0);
    public void SaveSelectedDronePref() => PlayerPrefs.SetInt(SelectedDroneSavePath, currentDroneIndex);
}

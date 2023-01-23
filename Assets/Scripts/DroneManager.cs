using DroneSim;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DroneManager : SingletonPersistent<DroneManager>
{
    public DroneSettingsSO[] drones;
    public int currentDroneIndex;
    public DroneSettingsSO CurrentDrone => drones[currentDroneIndex];

    public GameObject[] prefabs;
    public Sprite[] previewImages;
    public GameObject cameraManagerPrefab;

    private void Start()
    {
        // Carga cual fue el ultimo dron seleccionado
        LoadSelectedDronePref();

        // Cada vez que cargue un nivel destruye el anterior dron y spawnea uno nuevo
        GameManager.Instance.OnSceneLoaded += LoadDrone;
    }

    public void LoadDrone(Transform spawnPoint)
    {
        DestroyDrone();
        
        GameObject drone = Instantiate(CurrentDrone.prefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);
        drone.GetComponent<DroneController>().droneSettings = CurrentDrone;
    }

    public void LoadDrone()
    {
        GameObject parent = GameObject.FindWithTag("Player Parent");
        if (parent != null)
            LoadDrone(parent.transform);
    }

    public void DestroyDrone()
    {
        foreach (DroneController droneController in FindObjectsOfType<DroneController>())
        {
            Destroy(droneController.gameObject);
        }
    }

    public static readonly string SelectedDroneSavePath = "selected drone";
    private void LoadSelectedDronePref() => currentDroneIndex = PlayerPrefs.GetInt(SelectedDroneSavePath, 0);
    public void SaveSelectedDronePref() => PlayerPrefs.SetInt(SelectedDroneSavePath, currentDroneIndex);
    
}

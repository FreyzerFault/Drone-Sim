using DroneSim;
using UnityEngine;

public class DroneManager : SingletonPersistent<DroneManager>
{
    public DroneSettingsSO[] droneConfigurations;
    public int currentDroneConfigurationIndex;
    
    public DroneController currentDroneController;
    private DroneSettingsSO CurrentDroneSettings => droneConfigurations[currentDroneConfigurationIndex];

    protected override void Awake()
    {
        base.Awake();
        currentDroneController = FindObjectOfType<DroneController>();
    }

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
        
        GameObject drone = Instantiate(CurrentDroneSettings.prefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);

        currentDroneController = drone.GetComponent<DroneController>();
        currentDroneController.droneSettings = CurrentDroneSettings;
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
    private void LoadSelectedDronePref() => currentDroneConfigurationIndex = PlayerPrefs.GetInt(SelectedDroneSavePath, 0);
    public void SaveSelectedDronePref() => PlayerPrefs.SetInt(SelectedDroneSavePath, currentDroneConfigurationIndex);
    
}

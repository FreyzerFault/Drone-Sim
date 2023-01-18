using System;
using DroneSim;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DroneCameraManager : SingletonPersistent<DroneCameraManager>
{
    private int activeCamera = 0;
    public DroneCamera.CameraType initialCam = DroneCamera.CameraType.TPV;
    
    public DroneCamera[] cameras;
    public DroneCamera ActiveCamera => cameras[activeCamera];

    public DroneController dron;

    public event Action OnCameraSwitched;

    private void Start()
    {
        SceneManager.sceneLoaded += (scene, mode) =>
        {
            if (FindDron())
                LoadCamera();
            else
                Debug.Log("Dron not found");
        };

        cameras = GetComponentsInChildren<DroneCamera>();
        activeCamera = (int) initialCam;
        
        if (FindDron())
            LoadCamera();
    }

    private void Update()
    {
        if (cameras.Length == 0)
            cameras = GetComponentsInChildren<DroneCamera>(true);
    }

    public bool FindDron()
    {
        dron = FindObjectOfType<DroneController>();

        foreach (DroneCamera camera in cameras) camera.dron = dron;

        return dron != null;
    }

    public void SwitchCamera()
    {
        activeCamera = (activeCamera + 1) % cameras.Length;

        LoadCamera();

        OnCameraSwitched?.Invoke();
    }
    
    private void LoadCamera() => GameManager.Camera = ActiveCamera.camera;

    public Sprite ActiveCameraSprite => ActiveCamera.icon;
}

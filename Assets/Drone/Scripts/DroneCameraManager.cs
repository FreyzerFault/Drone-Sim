using System;
using DroneSim;
using UnityEngine;

public class DroneCameraManager : Singleton<DroneCameraManager>
{
    private int activeCamera = 0;
    public DroneCamera.CameraType initialCam = DroneCamera.CameraType.TPV;
    
    public DroneCamera[] cameras;
    public DroneCamera ActiveCamera => cameras[activeCamera];

    public DroneController dron;

    public event Action OnCameraSwitched;

    private void Start()
    {
        cameras = GetComponentsInChildren<DroneCamera>(true);
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
    
    private static void LoadCamera() => GameManager.Camera = Instance.ActiveCamera.camera;

    public Sprite ActiveCameraSprite => ActiveCamera.icon;
}

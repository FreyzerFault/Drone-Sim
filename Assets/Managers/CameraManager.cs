using System;
using System.Collections.Generic;
using System.Linq;
using DroneSim;
using UnityEngine;

public class CameraManager : SingletonPersistent<CameraManager>
{
    private DroneCamera.CameraType activeCamera = DroneCamera.CameraType.TPV;
    public DroneCamera.CameraType initialCam = DroneCamera.CameraType.TPV;
    
    public DroneCamera[] cameras;
    private Dictionary<DroneCamera.CameraType, DroneCamera> cameraMap = new Dictionary<DroneCamera.CameraType, DroneCamera>();

    public DroneCamera.CameraType ActiveCameraType => activeCamera;
    public DroneCamera ActiveCamera => cameraMap[activeCamera];

    public DroneController Dron => FindObjectOfType<DroneController>();

    public event Action OnCameraSwitched;

    private void Start()
    {
        InitializeCameras();

        GameManager.Instance.OnSceneLoaded += InitializeCameras;
    }

    public void SwitchCamera()
    {
        switch (activeCamera)
        {
            case DroneCamera.CameraType.FPV:
                LoadCamera(DroneCamera.CameraType.TPV);
                break;
            case DroneCamera.CameraType.TPV:
                LoadCamera(DroneCamera.CameraType.Static);
                break;
            case DroneCamera.CameraType.Static:
                LoadCamera(DroneCamera.CameraType.FPV);
                break;
        }

        OnCameraSwitched?.Invoke();
    }

    private void LoadCamera(DroneCamera.CameraType type)
    {
        activeCamera = type;
        
        foreach (DroneCamera droneCamera in cameras)
        {
            if (droneCamera.type == type)
                droneCamera.Enable();
            else
                droneCamera.Disable();
        }
    }

    private DroneCamera[] FindCamerasInScene() => cameras = FindObjectsOfType<DroneCamera>(true);

    private void InitializeCameras()
    {
        foreach (DroneCamera droneCamera in FindCamerasInScene())
        {
            droneCamera.Disable();
            
            if (!cameraMap.ContainsKey(droneCamera.type))
                cameraMap.Add(droneCamera.type, droneCamera);
        }

        activeCamera = initialCam;
        LoadCamera(initialCam);
    }
    
    public void DeleteCamera(DroneCamera camera)
    {
        List<DroneCamera> list = cameras.ToList();
        list.Remove(camera);
        cameras = list.ToArray();
        
        InitializeCameras();
    }
}

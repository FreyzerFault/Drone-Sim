using System;
using System.Collections.Generic;
using Cinemachine;
using DroneSim;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
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
        cameras = FindObjectsOfType<DroneCamera>(true);

        foreach (DroneCamera droneCamera in cameras)
        {
            droneCamera.Disable();
            
            if (!cameraMap.ContainsKey(droneCamera.type))
                cameraMap.Add(droneCamera.type, droneCamera);
            else
                Debug.LogError("Hay mas de una camara con el mismo tipo: " + droneCamera.name + " (" + droneCamera.type + ")");
        }

        activeCamera = initialCam;
        LoadCamera(initialCam);
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
}

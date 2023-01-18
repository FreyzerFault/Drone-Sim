using System;
using DroneSim;
using UnityEngine;

public abstract class DroneCamera : MonoBehaviour
{
    public enum CameraType
    {
        FPV = 0,
        TPV = 1,
        Static = 2,
    }

    public CameraType type;
    public int Index => (int) type;
    
    public Camera camera;
    public Sprite icon;

    public DroneController dron;

    private void Awake()
    {
        camera = GetComponent<Camera>();
        dron = FindObjectOfType<DroneController>();
    }

    protected abstract void OnEnable();

    protected abstract void LateUpdate();
}

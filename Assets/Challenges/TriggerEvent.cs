using System;
using DroneSim;
using UnityEngine;

public class TriggerEvent : MonoBehaviour
{
    public event Action OnTrigger;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<DroneController>()) OnTrigger?.Invoke();
    }
}

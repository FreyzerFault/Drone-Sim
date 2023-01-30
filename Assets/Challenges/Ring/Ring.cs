using System;
using DroneSim;
using UnityEngine;

public class Ring : MonoBehaviour
{
    public int id;
    private Animator animator;
    private static readonly int Pass = Animator.StringToHash("pass");

    private event Action<int> OnTrigger;


    public void SuscribeToTrigger(Action<int> onTrigger) => OnTrigger += onTrigger;

    private void Awake() => animator = GetComponent<Animator>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<DroneController>())
        {
            OnTrigger?.Invoke(id);
            if (animator != null)
                animator.SetTrigger(Pass);
        }
    }
}

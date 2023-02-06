using System;
using DroneSim;
using UnityEngine;

public class Ring : MonoBehaviour
{
    public int id;
    private bool on = true;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        particleSystem = GetComponentInChildren<ParticleSystem>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    #region Trigger
    
    private event Action<int> OnTrigger;
    public void SuscribeToTrigger(Action<int> onTrigger) => OnTrigger += onTrigger;
    
    private void OnTriggerEnter(Collider other)
    {
        // Pasa el dron y esta ON => Trigger
        if (other.GetComponentInParent<DroneController>() && on) Trigger();
    }
    
    private void Trigger()
    {
        OnTrigger?.Invoke(id);
        
        PlayAnimation();
        PlayParticles();
        TurnOff();
    }

    #endregion

    #region Is Visible Event
    
    public event Action OnVisible;
    public event Action OnInvisible;
    
    private void OnBecameVisible() => OnVisible?.Invoke();
    private void OnBecameInvisible() => OnInvisible?.Invoke();

    #endregion

    #region On / Off

    public void TurnOn()
    {
        on = true;
        meshRenderer.material = materialOn;
    }

    public void TurnOff()
    {
        on = false;
        meshRenderer.material = materialOff;
    }

    #endregion

    #region Animation

    private Animator animator;
    private static readonly int Pass = Animator.StringToHash("pass");
    
    private void PlayAnimation()
    {
        if (animator != null)
            animator.SetTrigger(Pass);
    }

    #endregion

    #region Particles

    private ParticleSystem particleSystem;

    private void PlayParticles()
    {
        if (particleSystem != null)
            particleSystem.Play();
    }

    #endregion

    #region Material

    private MeshRenderer meshRenderer;
    
    [SerializeField]
    private Material materialOn;
    [SerializeField]
    private Material materialOff;

    #endregion

}

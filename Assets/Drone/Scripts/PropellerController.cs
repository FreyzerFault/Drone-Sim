using UnityEngine;

public class PropellerController : MonoBehaviour
{
    public float enginePower;
    
    private Animator animator;
    
    private static readonly int EnginePower = Animator.StringToHash("EnginePower");

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Update()
    {
        animator.SetFloat(EnginePower, enginePower);
    }
}

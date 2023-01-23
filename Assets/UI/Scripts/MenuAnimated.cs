using UnityEngine;

public class MenuAnimated : Menu
{
    private Animator animator;

    public bool isPersistent = false;
    
    private static readonly int OpenID = Animator.StringToHash("open");
    
    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
        
        OnOpen += () => animator.SetBool(OpenID, true);
        OnClose += () => animator.SetBool(OpenID, false);

        if (!isPersistent)
            for (int i = 0; i < subMenus.Length; i++)
            {
                subMenus[i].OnClose += () => animator.SetBool(OpenID, true);
                subMenus[i].OnOpen += () => animator.SetBool(OpenID, false);
            }
    }
}

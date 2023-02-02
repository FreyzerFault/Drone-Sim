using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class Message : Singleton<Message>
{
    public bool openOnAwake = false;
    
    protected TMP_Text TMPtext;
    protected Image background;
    
    public bool IsOpen => animator.GetBool(OpenID);
    
    private Animator animator;
    private static readonly int OpenID = Animator.StringToHash("open");

    public string Text { get => TMPtext.text; set => TMPtext.text = value; }
    
    protected override void Awake()
    {
        base.Awake();
        
        TMPtext = GetComponentInChildren<TMP_Text>();
        background = GetComponentInChildren<Image>();
        animator = GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        if (openOnAwake)
            Open();
        else
            Close();
    }

    public virtual void Open()
    {
        animator.SetBool(OpenID, true);
        SetHeight();
    }
    
    public virtual void Close() => animator.SetBool(OpenID, false);
    
    private void SetHeight()
    {
        float backgroundWidth = background.rectTransform.sizeDelta.x;
        background.rectTransform.sizeDelta = new Vector2(backgroundWidth, TMPtext.preferredHeight);
    }

}

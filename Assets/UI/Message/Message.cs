using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class Message : Singleton<Message>
{
    public bool openOnAwake = false;
    
    private TMP_Text textComponent;
    private Image background;
    
    public bool IsOpen => animator.GetBool(OpenAnimID);
    
    private Animator animator;
    private static readonly int OpenAnimID = Animator.StringToHash("open");

    public string Text { get => textComponent.text; set => textComponent.text = value; }
    
    protected override void Awake()
    {
        base.Awake();
        
        textComponent = GetComponentInChildren<TMP_Text>();
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
        OpenAnimation();
        SetHeight();
    }

    protected virtual void Close() => CloseAnimation();

    private void OpenAnimation() => animator.SetBool(OpenAnimID, true);
    private void CloseAnimation() => animator.SetBool(OpenAnimID, false);
    
    private void SetHeight()
    {
        float backgroundWidth = background.rectTransform.sizeDelta.x;
        background.rectTransform.sizeDelta = new Vector2(backgroundWidth, textComponent.preferredHeight);
    }

}

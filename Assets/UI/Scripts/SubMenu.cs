using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class SubMenu : MonoBehaviour
{
    public bool isOpen = false;

    public bool saveSelected = false;
    public Selectable firstSelected;
    public Selectable parentSelected;

    public Selectable[] selectibles;
    public static Selectable CurrentSelected => EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
    
    private Animator animator;
    
    private static readonly int OpenID = Animator.StringToHash("open");

    public event Action OnOpen;
    public event Action OnClose;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();

        selectibles = GetComponentsInChildren<Selectable>();
    }

    public void Toggle()
    {
        if (isOpen)
            Close();
        else
            Open();
    }
    
    public void Open()
    {
        if (isOpen) return;
        isOpen = true;

        // Animacion
        animator.SetBool(OpenID, true);

        // Guardamos el seleccionado antes de abrir el menu para cuando se cierre seleccionarlo 
        parentSelected = CurrentSelected;
        
        // Selecciona el primero
        firstSelected.Select();
        
        OnOpen?.Invoke();
    }

    public virtual void Close()
    {
        if (!isOpen) return;
        isOpen = false;
        
        // Animacion
        animator.SetBool(OpenID, false);
        
        // Guarda el item seleccionado para la proxima vez que abra empezar por ahi
        if (saveSelected) firstSelected = CurrentSelected;
        
        // Seleccionamos el que ya habia seleccionado antes de abrirlo
        parentSelected.Select();
        
        OnClose?.Invoke();
    }

}

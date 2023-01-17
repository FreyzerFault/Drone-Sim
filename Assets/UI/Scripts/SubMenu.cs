using System;
using System.Collections.Generic;
using System.Linq;
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

    public List<Selectable> selectibles;
    
    public static Selectable CurrentSelected => EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
    public int SelectedIndex => selectibles.IndexOf(CurrentSelected);
    
    private Animator animator;
    
    private static readonly int OpenID = Animator.StringToHash("open");

    public event Action OnOpen;
    public event Action OnClose;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();

        selectibles = GetComponentsInChildren<Selectable>().ToList();
        
        // Carga el seleccionado por defecto
        firstSelected = selectibles[0];
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

    public virtual bool Close()
    {
        if (!isOpen) return true;
        isOpen = false;
        
        // Animacion
        animator.SetBool(OpenID, false);
        
        // Guarda el item seleccionado para la proxima vez que abra empezar por ahi
        if (saveSelected) firstSelected = CurrentSelected;

        // Seleccionamos el que ya habia seleccionado antes de abrirlo
        parentSelected.Select();
        
        OnClose?.Invoke();

        return true;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public List<Menu> subMenus = new List<Menu>();
    [HideInInspector] public int menuOpened = -1;
    
    [HideInInspector] public bool isOpen = false;
    public bool closeOnCancel = true;

    public bool saveSelected = false;
    public Selectable firstSelected;
    public Selectable parentSelected;

    public List<Selectable> selectibles;
    
    public static Selectable CurrentSelected => (EventSystem.current.currentSelectedGameObject == null) ? null : EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
    public int SelectedIndex => selectibles.IndexOf(CurrentSelected);

    public event Action BeforeOpen;
    public event Action OnOpen;
    public event Action BeforeClose;
    public event Action OnClose;

    #region Animator

    private Animator animator;
    
    private static readonly int OpenID = Animator.StringToHash("open");
    
    public bool isPersistent = false;

    #endregion

    protected virtual void Awake()
    {
        isOpen = false;
        
        selectibles = GetComponentsInChildren<Selectable>().ToList();

        // Carga el seleccionado por defecto
        firstSelected = selectibles[0];
    }

    protected virtual void Start()
    {
        for (int i = 0; i < subMenus.Count; i++)
        {
            int menuID = i;
            subMenus[i].OnClose += () => menuOpened = -1;
            subMenus[i].OnOpen += () => menuOpened = menuID;
        }
        
        // Animation
        animator = GetComponent<Animator>();
        
        if (animator != null && !isPersistent)
            for (int i = 0; i < subMenus.Count; i++)
            {
                subMenus[i].OnClose += () => animator.SetBool(OpenID, true);
                subMenus[i].OnOpen += () => animator.SetBool(OpenID, false);
            }
    }

    public void Toggle()
    {
        if (isOpen)
            Close();
        else
            Open();
    }

    public virtual void Open()
    {
        if (isOpen) return;
        
        BeforeOpen?.Invoke();
        
        isOpen = true;
        
        // Guardamos el seleccionado antes de abrir el menu para cuando se cierre seleccionarlo 
        parentSelected = CurrentSelected;
        
        // Selecciona el primero
        if (firstSelected)
            firstSelected.Select();
        
        if (animator != null)
            animator.SetBool(OpenID, true);
        else
            gameObject.SetActive(true);

        OnOpen?.Invoke();
    }

    public virtual bool Close()
    {
        if (!isOpen) return true;

        BeforeClose?.Invoke();
        
        if (menuOpened != -1)
            if (!CloseSubMenu())
                return false;
        
        isOpen = false;

        // Guarda el item seleccionado para la proxima vez que abra empezar por ahi
        if (saveSelected) firstSelected = CurrentSelected;

        // Seleccionamos el que ya habia seleccionado antes de abrirlo
        if (parentSelected != null)
            parentSelected.Select();
        
        if (animator != null)
            animator.SetBool(OpenID, false);
        else
            gameObject.SetActive(false);

        OnClose?.Invoke();

        return true;
    }
    
    // Abre o Cierra un submenu
    public virtual void ToggleSubMenu(int menuID)
    {
        // Si ya estaba abierto lo cierra
        if (menuOpened == menuID)
        {
            CloseSubMenu();
            return;
        }
        
        // Si no cerramos si hay alguno otro abierto y abrimos el menu
        if (CloseSubMenu())
            OpenSubMenu(menuID);
    }
    
    protected bool CloseSubMenu()
    {
        if (menuOpened == -1) return true;
        return subMenus[menuOpened].Close();
    }
    
    protected void OpenSubMenu(int menuID)
    {
        if (menuOpened == menuID) return;
        menuOpened = menuID;
        subMenus[menuID].Open();
    }

    public virtual void OnCancelRecursive()
    {
        if (isOpen)
        {
            if (menuOpened != -1)
                subMenus[menuOpened].OnCancelRecursive();
            else if (closeOnCancel)
                Close();
        }
    }
}

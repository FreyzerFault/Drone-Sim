using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class Menu : MonoBehaviour
{
    public Menu[] subMenus;
    protected int menuOpened = -1;
    
    [HideInInspector] public bool isOpen = false;

    public bool saveSelected = false;
    public Selectable firstSelected;
    public Selectable parentSelected;

    public List<Selectable> selectibles;
    
    public static Selectable CurrentSelected => (EventSystem.current.currentSelectedGameObject == null) ? null : EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
    public int SelectedIndex => selectibles.IndexOf(CurrentSelected);

    public event Action OnOpen;
    public event Action OnClose;

    protected virtual void Awake()
    {
        selectibles = GetComponentsInChildren<Selectable>().ToList();

        // Carga el seleccionado por defecto
        firstSelected = selectibles[0];
    }

    private void Start()
    {
        for (int i = 0; i < subMenus.Length; i++)
        {
            int menuID = i;
            subMenus[i].OnClose += () => menuOpened = -1;
            subMenus[i].OnOpen += () => menuOpened = menuID;
        }
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
        
        // Guardamos el seleccionado antes de abrir el menu para cuando se cierre seleccionarlo 
        parentSelected = CurrentSelected;
        
        // Selecciona el primero
        if (firstSelected)
            firstSelected.Select();

        OnOpen?.Invoke();
    }

    public virtual bool Close()
    {
        if (!isOpen) return true;

        if (menuOpened != -1)
            if (!CloseSubMenu())
                return false;
        
        isOpen = false;

        // Guarda el item seleccionado para la proxima vez que abra empezar por ahi
        if (saveSelected) firstSelected = CurrentSelected;

        // Seleccionamos el que ya habia seleccionado antes de abrirlo
        if (parentSelected != null)
            parentSelected.Select();
        
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
        subMenus[menuID].Open();
    }

    public void OnCancelRecursive()
    {
        if (menuOpened == -1)
            Close();
        else
            subMenus[menuOpened].OnCancelRecursive();
    }
}

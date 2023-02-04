using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [HideInInspector] public bool isOpen = false;
    public bool closeOnCancel = true;

    public event Action BeforeOpen;
    public event Action OnOpen;
    public event Action BeforeClose;
    public event Action OnClose;


    protected virtual void Awake()
    {
        selectibles = GetComponentsInChildren<Selectable>().ToList();

        // Carga el seleccionado por defecto
        if (firstSelected == null)
            firstSelected = selectibles[0];
    }

    protected virtual void Start()
    {
        HandleSubmenusEvents();
        
        InitializeEventsAnimations();
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
        
        // Guarda el anterior seleccionable para cuando se cierre el menu volverlo a seleccionar
        SaveParentSelectable();
        
        // Selecciona el primero
        SelectFirstSelectable();
        
        OpenAnimation();

        OnOpen?.Invoke();
    }

    public virtual bool Close()
    {
        if (!isOpen) return true;

        BeforeClose?.Invoke();
        
        if (submenuOpened != -1)
            if (!CloseSubMenu())
                return false;
        
        isOpen = false;

        // Guarda el item seleccionado para la proxima vez que abra empezar por ahi
        if (saveSelected) SaveCurrentSelected();

        // Seleccionamos el que ya habia seleccionado antes de abrirlo
        SelectParentSelectable();
        
        CloseAnimation();

        OnClose?.Invoke();

        return true;
    }

    #region Selectables

    public bool saveSelected = false;
    public Selectable firstSelected;
    public Selectable parentSelected;

    public List<Selectable> selectibles;

    private static Selectable CurrentSelected => (EventSystem.current.currentSelectedGameObject == null) ? null : EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
    public int SelectedIndex => selectibles.IndexOf(CurrentSelected);
    
    // Guarda el item seleccionado antes de abrir el menu para cuando se cierre seleccionarlo de vuelta
    private void SaveParentSelectable() => parentSelected = CurrentSelected;

    private void SaveCurrentSelected() => firstSelected = CurrentSelected;
    
    private void SelectFirstSelectable()
    {
        if (firstSelected)
            firstSelected.Select();
    }
    
    private void SelectParentSelectable()
    {
        if (parentSelected != null)
            parentSelected.Select();
    }
    
    

    #endregion

    #region Animations
    
    private Animator animator;
    
    // Si un menu es PERSISTENTE se mantiene abierto visualmente 
    // Si no es persistente se cierra visualmente el menu cuando se abre un submenu
    [SerializeField] private bool isPersistent = false;
    
    private static readonly int OpenID = Animator.StringToHash("open");

    private void OpenAnimation()
    {
        if (animator != null)
            animator.SetBool(OpenID, true);
        else if (!gameObject.activeSelf)
            gameObject.SetActive(true);
    }
    
    private void CloseAnimation()
    {
        if (animator != null)
            animator.SetBool(OpenID, false);
        else if (gameObject.activeSelf)
            gameObject.SetActive(false);
    }
    
    private void InitializeEventsAnimations()
    {
        
        // Animation
        animator = GetComponent<Animator>();
        
        // Si un menu es PERSISTENTE se mantiene abierto visualmente 
        // Si no es persistente se cierra visualmente el menu cuando se abre un submenu
        if (animator != null && !isPersistent)
            for (int i = 0; i < subMenus.Count; i++)
            {
                subMenus[i].OnClose += OpenAnimation;
                subMenus[i].OnOpen += CloseAnimation;
            }
    }
    
    #endregion

    #region Submenus

    [SerializeField] protected List<Menu> subMenus = new List<Menu>();
    [HideInInspector] public int submenuOpened = -1;
    
    protected Menu SubmenuOpened => IsAnySubmenuOpened ? subMenus[submenuOpened] : null;
    public bool IsAnySubmenuOpened => submenuOpened != -1;
    public bool IsLastSubmenuOpened => submenuOpened == SubmenusCount - 1;
    public bool IsFirstSubmenuOpened => submenuOpened == 0;
    public int SubmenusCount => subMenus.Count;

    public Menu GetSubmenu(int index) => subMenus.Count > index ? subMenus[index] : null;
    
    private void HandleSubmenusEvents()
    {
        for (int i = 0; i < subMenus.Count; i++)
        {
            int menuID = i;
            
            // Modificamos el indice que marca que submenu hay abierto
            subMenus[i].OnClose += () => submenuOpened = -1;
            subMenus[i].OnOpen += () => submenuOpened = menuID;
        }
    }
    

    // Abre o Cierra un submenu
    public virtual void ToggleSubMenu(int menuID)
    {
        // Si ya estaba abierto lo cierra
        if (submenuOpened == menuID)
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
        if (submenuOpened == -1) return true;
        return subMenus[submenuOpened].Close();
    }
    
    protected void OpenSubMenu(int menuID)
    {
        if (submenuOpened == menuID) return;
        submenuOpened = menuID;
        subMenus[menuID].Open();
    }

    #endregion
    
    

    public virtual void OnCancelRecursive()
    {
        if (isOpen)
        {
            if (IsAnySubmenuOpened)
                SubmenuOpened.OnCancelRecursive();
            else if (closeOnCancel)
                Close();
        }
    }
}

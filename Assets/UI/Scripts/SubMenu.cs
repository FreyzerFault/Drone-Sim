using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))] [ExecuteAlways]
public class SubMenu : MonoBehaviour
{
    public bool isOpen = false;

    public bool saveSelected = false;
    public Selectable firstSelected;
    public Selectable parentSelected;

    public Selectable[] selectibles;
    public Selectable CurrentSelected => EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
    
    private Animator animator;
    
    private static readonly int OpenID = Animator.StringToHash("open");

    public event Action OnOpen;
    public event Action OnClose;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        
        // Primer Item seleccionado
        firstSelected.Select();

        selectibles = GetComponentsInChildren<Selectable>();
        SetCancelCallbacksOnSelectibles();
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

    public void Close()
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

    // Pone a todos los items seleccionables un callback para el boton Cancel que cierra el menu
    private void SetCancelCallbacksOnSelectibles()
    {
        foreach (Selectable selectible in selectibles)
        {
            EventTrigger eventTrigger = selectible.GetComponent<EventTrigger>();
            if (eventTrigger == null)
                eventTrigger = selectible.gameObject.AddComponent<EventTrigger>();
            
            eventTrigger.triggers.Clear();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.Cancel;
            entry.callback = new EventTrigger.TriggerEvent();
            entry.callback.AddListener(new UnityEngine.Events.UnityAction<BaseEventData>(OnCancel));
            eventTrigger.triggers.Add(entry);
        }
    }

    public void OnCancel(BaseEventData ed)
    {
        Debug.Log("Cancel");
        Close();
    }
}

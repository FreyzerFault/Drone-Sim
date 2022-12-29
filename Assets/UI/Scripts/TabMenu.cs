using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TabMenu : SubMenu
{
    public int selectedTab;
    public Color selectedTabColor;
    private int TabCount => tabButtons.Count;


    public GameObject tabsParent;
    public GameObject menusParent;
    
    [SerializeField] protected List<Button> tabButtons = new List<Button>();
    [SerializeField] protected List<GameObject> menus = new List<GameObject>();

    protected event Action onTabChange;


    protected override void Awake()
    {
        base.Awake();
        
        tabButtons = tabsParent.GetComponentsInChildren<Button>().ToList();
        
        for (int i = 0; i < TabCount; i++)
        {
            menus.Add(menusParent.transform.GetChild(i).gameObject);
            menus[i].SetActive(false);
        }

        OnOpen += () => OpenTab(0);
    }

    public void OnTabNavigate(InputValue value)
    {
        // Esta seleccionado el ultimo y pulsa derecha
        if (value.Get<float>() > 0 && selectedTab == TabCount - 1)
            return;
        
        // Esta seleccionado el primero y pulsa izquierda
        if (value.Get<float>() < 0 && selectedTab == 0)
            return;
        
        OpenTab(selectedTab + (int)value.Get<float>());
    }

    public void OpenTab(int newTab)
    {
        // Cambia el color de la pestaña
        tabButtons[selectedTab].GetComponent<Image>().color = Color.white;
        tabButtons[newTab].GetComponent<Image>().color = selectedTabColor;
        
        // Cierra la anterior y abre esta
        menus[selectedTab].SetActive(false);
        menus[newTab].SetActive(true);
        
        // Selecciona el primer item seleccionable
        menus[newTab].GetComponentsInChildren<Selectable>()[0].Select();

        selectedTab = newTab;
        
        onTabChange?.Invoke();
    }
}

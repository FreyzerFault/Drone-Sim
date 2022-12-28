using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TabsController : MonoBehaviour
{
    public int selectedTab;
    public int TabCount => GetComponentsInChildren<Button>().Length;


    public List<Button> buttons;
    public List<GameObject> tabMenus;

    public EventSystem tabEventSystem;

    private void Awake()
    {
        buttons = GetComponentsInChildren<Button>().ToList();

        foreach (GameObject menu in tabMenus) menu.SetActive(false);
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

    public void OpenTab(int tab)
    {
        // Cerrar la anterior
        tabMenus[selectedTab].SetActive(false);
        tabMenus[tab].SetActive(true);
        
        GetComponentsInChildren<Button>()[tab].GetComponent<Image>().color = Color.yellow;
        
        selectedTab = tab;
    }
}

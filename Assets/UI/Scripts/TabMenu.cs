using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TabMenu : MonoBehaviour
{
    [SerializeField] protected List<Button> buttons = new List<Button>();
    public Color selectedTabColor;
    public GameObject tabsParent;

    private Menu menu;

    protected void Awake()
    {
        menu = GetComponent<Menu>();
        buttons = tabsParent.GetComponentsInChildren<Button>().ToList();
    }

    public void OpenTab(int tabIndex)
    {
        if (!menu.isOpen)
            menu.Open();

        if (menu.menuOpened == tabIndex) return;

        foreach (Button button in buttons) 
            button.GetComponent<Image>().color = Color.white;

        buttons[tabIndex].GetComponent<Image>().color = selectedTabColor;
        menu.ToggleSubMenu(tabIndex);
    }

    public void OnTabNavigate(InputValue value)
    {
        // Esta seleccionado el ultimo y pulsa derecha
        if (value.Get<float>() > 0 && menu.menuOpened == menu.subMenus.Count - 1)
            return;
        
        // Esta seleccionado el primero y pulsa izquierda
        if (value.Get<float>() < 0 && menu.menuOpened == 0)
            return;
        
        OpenTab(menu.menuOpened + (int)value.Get<float>());
    }
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TabMenu : MonoBehaviour
{
    [SerializeField] private Color selectedTabColor;
    
    [SerializeField] private GameObject tabsParent;
    [SerializeField] private List<Button> buttons = new List<Button>();

    private Menu menu;

    private void Awake()
    {
        TryGetComponent(out menu);
        buttons = tabsParent.GetComponentsInChildren<Button>().ToList();
    }

    public void OpenTab(int tabIndex)
    {
        // No esta el menu abierto => Lo abre
        if (!menu.isOpen)
            menu.Open();

        // Ya estaba la tab abierta => no hace nada
        if (menu.submenuOpened == tabIndex) return;
        
        SelectTabButton(tabIndex);
        
        menu.ToggleSubMenu(tabIndex);
    }

    // INPUT reservado para cambiar de tab
    private void OnTabNavigate(InputValue value)
    {
        float input = value.Get<float>();
        if (input == 0) return;
        
        // Esta seleccionado el ultimo y pulsa derecha
        if (input > 0 && menu.IsLastSubmenuOpened)
            return;
        
        // Esta seleccionado el primero y pulsa izquierda
        if (input < 0 && menu.IsFirstSubmenuOpened)
            return;
        
        OpenTab(menu.submenuOpened + (int) input);
    }

    private void SelectTabButton(int tabIndex)
    {
        foreach (Button button in buttons)
            button.GetComponent<Image>().color = Color.white;

        buttons[tabIndex].GetComponent<Image>().color = selectedTabColor;
    }
}

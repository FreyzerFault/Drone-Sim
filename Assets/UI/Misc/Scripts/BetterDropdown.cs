using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(TMP_Dropdown))]
public class BetterDropdown : MonoBehaviour
{ 
    protected TMP_Dropdown Dropdown;
    private Scrollbar Scrollbar;

    private void Awake()
    {
        Dropdown = GetComponent<TMP_Dropdown>();
        Scrollbar = GetComponentInChildren<Scrollbar>();
    }

    private void LateUpdate()
    {
        if (!Dropdown.IsExpanded) return;
        
        ScrollToSelectedOption();
    }

    private void ScrollToSelectedOption()
    {
        if (Scrollbar == null)
            Scrollbar = GetComponentInChildren<Scrollbar>();
        
        GameObject selected = EventSystem.current.currentSelectedGameObject;
        
        // Dropdown names are like "Item 5: 1920x1080 60Hz"
        // Separamos el numero del "Item 5" para saber donde esta
        if (selected == null || !selected.name.Contains(":")) return;
        string itemName = selected.name.Split(":")[0];
        string itemNumber = itemName.Split(" ")[1];
        
        if (itemNumber.Length == 0) return;
        
        Scrollbar.value = Mathf.InverseLerp(Dropdown.options.Count - 1, 0, int.Parse(itemNumber));
    }

    protected void UpdateDropdownOptions(List<string> options, int selectedIndex)
    {
        if (Dropdown == null && !TryGetComponent(out Dropdown)) return;

        Dropdown.ClearOptions();
        Dropdown.AddOptions(options);
        Dropdown.value = selectedIndex;
        Dropdown.RefreshShownValue();
    }
}

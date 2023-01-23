using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DynamicNavigation : MonoBehaviour
{
    public GameObject topSection;
    public GameObject downSection;
    public GameObject leftSection;
    public GameObject rightSection;

    private List<Selectable> selectables;

    private void Awake() => selectables = GetComponentsInChildren<Selectable>().ToList();

    public void UpdateNavigation()
    {
        Selectable topSelectable = null, downSelectable = null, leftSelectable = null, rightSelectable = null;

        if (topSection != null)
            topSelectable = GetNearestSelectableTop();
        if (downSection != null)
            downSelectable = GetNearestSelectableDown();
        if (leftSection != null) 
            leftSelectable = GetNearestSelectableLeft();
        if (rightSection != null)
            rightSelectable = GetNearestSelectableRight();

        if (topSelectable != null)
            UpdateTopSelectable(topSelectable);
        if (downSelectable != null)
            UpdateDownSelectable(downSelectable);
        if (leftSelectable != null)
            UpdateLeftSelectable(leftSelectable);
        if (rightSelectable != null)
            UpdateRightSelectable(rightSelectable);
    }

    private void UpdateTopSelectable(Selectable nearestSelectable)
    {
        Navigation nearestNav = nearestSelectable.navigation;
        nearestNav.selectOnDown = selectables[0];
        nearestSelectable.navigation = nearestNav;
        
        foreach (Selectable selectable in selectables)
        {
            Navigation nav = selectable.navigation;
            nav.selectOnUp = nearestSelectable;
            selectable.navigation = nav;
        }
    }
    private void UpdateDownSelectable(Selectable nearestSelectable)
    {
        Navigation nearestNav = nearestSelectable.navigation;
        nearestNav.selectOnUp = selectables[^1];
        nearestSelectable.navigation = nearestNav;
        
        foreach (Selectable selectable in selectables)
        {
            Navigation nav = selectable.navigation;
            nav.selectOnDown = nearestSelectable;
            selectable.navigation = nav;
        }
    }
    private void UpdateLeftSelectable(Selectable nearestSelectable)
    {
        Navigation nearestNav = nearestSelectable.navigation;
        nearestNav.selectOnRight = selectables[0];
        nearestSelectable.navigation = nearestNav;
        
        foreach (Selectable selectable in selectables)
        {
            Navigation nav = selectable.navigation;
            nav.selectOnLeft = nearestSelectable;
            selectable.navigation = nav;
        }
    }
    private void UpdateRightSelectable(Selectable nearestSelectable)
    {
        Navigation nearestNav = nearestSelectable.navigation;
        nearestNav.selectOnLeft = selectables[0];
        nearestSelectable.navigation = nearestNav;
        
        foreach (Selectable selectable in selectables)
        {
            Navigation nav = selectable.navigation;
            nav.selectOnRight = nearestSelectable;
            selectable.navigation = nav;
        }
    }

    private Selectable GetNearestSelectableTop() => topSection.GetComponentsInChildren<Selectable>()[^1];
    private Selectable GetNearestSelectableDown() => downSection.GetComponentsInChildren<Selectable>()[0];
    
    private Selectable GetNearestSelectableLeft()
    {
        List<float> distancesHorizontal = new List<float>();
        List<Selectable> leftSelectables = leftSection.GetComponentsInChildren<Selectable>().ToList();
            
        foreach (Selectable selectable in leftSelectables)
            distancesHorizontal.Add(Mathf.Abs(GetComponent<RectTransform>().rect.xMin - selectable.transform.position.x));

        float min = 99999;
        int index = 0;
        for (int i = 0; i < distancesHorizontal.Count; i++)
            if (distancesHorizontal[i] < min)
            {
                index = i;
                min = distancesHorizontal[i];
            }

        return leftSelectables[index];
    }
    
    private Selectable GetNearestSelectableRight()
    {
        List<float> distancesHorizontal = new List<float>();
        List<Selectable> rightSelectables = rightSection.GetComponentsInChildren<Selectable>().ToList();
            
        foreach (Selectable selectable in rightSelectables)
            distancesHorizontal.Add(Mathf.Abs(selectable.transform.position.x - GetComponent<RectTransform>().rect.xMax));

        float min = 99999;
        int index = 0;
        for (int i = 0; i < distancesHorizontal.Count; i++)
            if (distancesHorizontal[i] < min)
            {
                index = i;
                min = distancesHorizontal[i];
            }

        return rightSelectables[index];
    }
}

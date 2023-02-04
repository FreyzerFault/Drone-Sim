using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DynamicNavigation : MonoBehaviour
{
    [SerializeField] private GameObject topSection;
    [SerializeField] private GameObject bottomSection;
    [SerializeField] private GameObject leftSection;
    [SerializeField] private GameObject rightSection;

    private List<Selectable> selectables;

    private void Awake() => selectables = GetComponentsInChildren<Selectable>().ToList();

    // Use this to Change a section in each direction so it can update navigation dynamicaly to the new section 
    public void UpdateTopSection(GameObject newTopSection)
    {
        topSection = newTopSection;
        UpdateNavigation();
    }
    public void UpdateBottomSection(GameObject newBottomSection)
    {
        bottomSection = newBottomSection;
        UpdateNavigation();
    }
    public void UpdateLeftSection(GameObject newLeftSection)
    {
        leftSection = newLeftSection;
        UpdateNavigation();
    }
    public void UpdateRightSection(GameObject newRightSection)
    {
        rightSection = newRightSection;
        UpdateNavigation();
    }
    
    

    #region Update Navigation to a new Nearest Selectible in each Direction

    private void UpdateNavigation()
    {
        Selectable topSelectable = null, botSelectable = null, leftSelectable = null, rightSelectable = null;

        if (topSection != null)
            topSelectable = GetNearestSelectableTop();
        if (bottomSection != null)
            botSelectable = GetNearestSelectableBot();
        if (leftSection != null) 
            leftSelectable = GetNearestSelectableLeft();
        if (rightSection != null)
            rightSelectable = GetNearestSelectableRight();

        if (topSelectable != null)
            UpdateTopSelectable(topSelectable);
        if (botSelectable != null)
            UpdateBotSelectable(botSelectable);
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
    private void UpdateBotSelectable(Selectable nearestSelectable)
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

    #endregion

    #region Find Nearest Selectable in each Direction

    private Selectable GetNearestSelectableTop() => topSection.GetComponentsInChildren<Selectable>()[^1];
    private Selectable GetNearestSelectableBot() => bottomSection.GetComponentsInChildren<Selectable>()[0];
    
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

    #endregion
}

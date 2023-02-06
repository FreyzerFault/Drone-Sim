using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(TMP_Dropdown))]
public class ResolutionDropdown : BetterDropdown
{
    private Resolution[] resolutions;
    private int refreshRate;

    [HideInInspector] public int resolutionIndex;

    private Resolution CurrentResolution => resolutions[resolutionIndex];


    private void Start()
    {
        UpdateDropdownOptions();
        InitializeValueChangedEvents();
    }

    public void UpdateDropdownOptions()
    {
        refreshRate = ResolutionManager.CurrentMonitorRefreshRate;
        resolutions = ResolutionManager.FilteredResolutionsByRefreshRate(refreshRate);

        List<string> options = new List<string>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            Resolution resolution = resolutions[i];
            options.Add(resolution.width + "x" + resolution.height + " " + resolution.refreshRate + " Hz");
            if (resolution.width == ResolutionManager.Instance.Resolution.x &&
                resolution.height == ResolutionManager.Instance.Resolution.y)
                resolutionIndex = i;
        }

        base.UpdateDropdownOptions(options, resolutionIndex);
    }

    private void InitializeValueChangedEvents()
    {
        if (Dropdown == null && !TryGetComponent(out Dropdown)) return;

        Dropdown.onValueChanged.AddListener(
            index =>
            {
                resolutionIndex = index;
                UpdateResolution();
            });
    }

    private void UpdateResolution()
    {
        ResolutionManager.SetResolution(resolutions[resolutionIndex]);
        ResolutionManager.SetRefreshRate(refreshRate);
    }
}
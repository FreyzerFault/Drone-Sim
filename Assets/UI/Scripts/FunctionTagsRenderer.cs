using System;
using System.Collections.Generic;
using System.Globalization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class FunctionTagsRenderer : MonoBehaviour
{
    public GameObject functionTagPrefab;

    public Transform verticalNumsContainer;
    public Transform verticalTag;
    public Color colorVertical;
    public List<float> verticalNums;

    public Transform horizontalNumsContainer;
    public Transform horizontalTag;
    public Color colorHorizontal;
    public List<float> horizontalNums;
    
    public Vector2Int numsSkipped = Vector2Int.zero;

    private FunctionRenderer functionRenderer;
    
    private Vector2 RangeX => functionRenderer.rangeX;
    private Vector2 RangeY => functionRenderer.rangeY;
    
    private Vector2Int numsTags;

    private void Awake()
    {
        functionRenderer = GetComponent<FunctionRenderer>();
    }

    public void Start()
    {
        Vector2Int gridSize = functionRenderer.gridSize;
        
        if (numsSkipped.x < 0) numsSkipped.x = 0;
        if (numsSkipped.y < 0) numsSkipped.y = 0;
        
        if (numsSkipped.x > gridSize.x / 2) numsSkipped.x = gridSize.x / 2;
        if (numsSkipped.y > gridSize.y / 2) numsSkipped.y = gridSize.y / 2;

        if (gridSize.x % (numsSkipped.x + 1) != 0) numsSkipped.x = numsSkipped.x + 1 - gridSize.x % (numsSkipped.x + 1);

        numsTags.x = gridSize.x / (numsSkipped.x + 1) + 1;
        numsTags.y = gridSize.y / (numsSkipped.y + 1) + 1;
        
        if (verticalNumsContainer != null)
            SetVerticalNums();
        if (horizontalNumsContainer != null)
            SetHorizontalNums();

        SetColors();
    }

    private void SetColors()
    {
        if (verticalNumsContainer != null)
            foreach (Transform child in verticalNumsContainer)
                child.GetComponent<Text>().color = colorVertical;
        if (horizontalNumsContainer != null)
            foreach (Transform child in horizontalNumsContainer)
                child.GetComponent<Text>().color = colorHorizontal;

        if (verticalTag != null) verticalTag.GetComponent<Text>().color = colorVertical;
        if (horizontalTag != null) horizontalTag.GetComponent<Text>().color = colorHorizontal;
    }

    private void SetVerticalNums()
    {
        verticalNumsContainer.transform.ClearChildren();

        float height = functionRenderer.grid.GetComponent<RectTransform>().rect.height;
        float numHeight = functionTagPrefab.GetComponent<RectTransform>().rect.height;
        verticalNumsContainer.GetComponent<VerticalLayoutGroup>().spacing = (height + numHeight * (1f - numsTags.y)) / (numsTags.y - 1f);
        
        float intervalY = (RangeY.y - RangeY.x) / (numsTags.y - 1f);
        for (int i = 0; i < numsTags.y; i++)
        {
            GameObject numObj = Instantiate(functionTagPrefab, verticalNumsContainer);
            numObj.GetComponent<Text>().text = (RangeY.y - i * intervalY).ToString("0.##");
        }
    }

    private void SetHorizontalNums()
    {
        horizontalNumsContainer.transform.ClearChildren();
        
        float width = functionRenderer.grid.GetComponent<RectTransform>().rect.width;
        float numWidth = functionTagPrefab.GetComponent<RectTransform>().rect.width;
        horizontalNumsContainer.GetComponent<HorizontalLayoutGroup>().spacing = (width + numWidth * (1f - numsTags.x)) / (numsTags.x - 1f);
        
        float intervalX = (RangeX.y - RangeX.x) / (numsTags.x - 1f);
        for (int i = 0; i < numsTags.x; i++)
        {
            GameObject numObj = Instantiate(functionTagPrefab, horizontalNumsContainer);
            numObj.GetComponent<Text>().text = (RangeX.x + i * intervalX).ToString("0.##");
        }
    }
}

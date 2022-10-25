using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

[ExecuteAlways]
public class FunctionRenderer : MonoBehaviour
{
    public UIGridRenderer grid;
    public UILineRenderer line;
    
    public Vector2Int gridSize = new Vector2Int(10, 10);
    public float gridThickness = 1;
    public float lineThickness = 1;

    public List<Vector2> Points => line.points;
    
    public Vector2 rangeX = new Vector2(-1, 1);
    public Vector2 rangeY = new Vector2(-1, 1);
    
    public float dx = 1;
    public float dy = 1;

    public void Start()
    {
        if (grid == null) grid = GetComponentInChildren<UIGridRenderer>();
        if (line == null) line = GetComponentInChildren<UILineRenderer>();

        grid.gridSize = gridSize;
        
        grid.thickness = gridThickness;
        line.thickness = lineThickness;
        
        RectTransform rectT = GetComponent<RectTransform>();
        grid.width = line.width = rectT.rect.width;
        grid.height = line.height = rectT.rect.height;

        line.rangeX = rangeX;
        line.rangeY = rangeY;
        
        PlotFunction(x => x * x * x * x * x);
    }

    private void Update()
    {
        grid.gridSize = gridSize;
        
        grid.thickness = gridThickness;
        line.thickness = lineThickness;
        
        RectTransform rectT = GetComponent<RectTransform>();
        grid.width = line.width = rectT.rect.width;
        grid.height = line.height = rectT.rect.height;

        line.rangeX = rangeX;
        line.rangeY = rangeY;

        line.dx = dx;
        line.dy = dy;
    }

    public void PlotPoint(Vector2 point) => line.PlotPoint(point);
    public void PlotPoint(float x, float y) => line.PlotPoint(new Vector2(x,y));
    
    public void PlotFunction(Func<float, float> function)
    {
        line.Clear();
        
        for (float x = rangeX.x; x <= rangeX.y; x += dx)
        {
            float y = function(x);
            PlotPoint(x,y);
        }
    }
    
    
    private void GenerateRandomLine()
    {
        line.Clear();
        
        for (int i = 0; i < rangeX.y - rangeX.x; i++)
        {
            Vector3 p = new Vector3(i + rangeX.x, Mathf.Lerp(rangeY.x, rangeY.y , Random.value));
            PlotPoint(p);
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
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

    [HideInInspector] public FunctionTagsRenderer tagRenderer;

    private void Awake()
    {
        tagRenderer = GetComponent<FunctionTagsRenderer>();
        
        if (grid == null) grid = GetComponentInChildren<UIGridRenderer>();
        if (line == null) line = GetComponentInChildren<UILineRenderer>();
    }

    public void Start()
    {
        grid.gridSize = gridSize;
        
        grid.thickness = gridThickness;
        line.thickness = lineThickness;
        
        Rect rect = grid.GetComponent<RectTransform>().rect;
        grid.width = line.width = rect.width;
        grid.height = line.height = rect.height;

        line.rangeX = rangeX;
        line.rangeY = rangeY;

        line.dx = dx;
        line.dy = dy;
        
        //PlotFunction(x => -4 + 2*x + 1/2 * 2 * x * x);
    }

    private void Update()
    {
        // grid.gridSize = gridSize;
        //
        // grid.thickness = gridThickness;
        // line.thickness = lineThickness;
        //
        // RectTransform rectT = GetComponent<RectTransform>();
        // grid.width = line.width = rectT.rect.width;
        // grid.height = line.height = rectT.rect.height;
        //
        // line.rangeX = rangeX;
        // line.rangeY = rangeY;
        //
        // line.dx = dx;
        // line.dy = dy;
    }

    public void PlotPoint(Vector2 point) => line.PlotPoint(point);
    public void PlotPoint(float x, float y) => line.PlotPoint(new Vector2(x,y));
    
    public void PlotFunction(Func<float, float> function)
    {
        line.Clear();

        if (dx <= 0) return;
        
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

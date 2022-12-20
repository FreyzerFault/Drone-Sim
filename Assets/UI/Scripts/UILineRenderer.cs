using System;
using System.Collections.Generic;
using DroneSim;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UILineRenderer : Graphic
{
    public List<Vector2> points;

    public float width;
    public float height;
    
    public float thickness = 10f;
    
    [HideInInspector] public Vector2 rangeX = new Vector2(-1, 1);
    [HideInInspector] public Vector2 rangeY = new Vector2(-1, 1);

    [HideInInspector] public float dx = 1;
    [HideInInspector] public float dy = 1;

    public void PlotPoint(Vector2 point)
    {
        if (!InRange(point))
            return;
        
        Vector2 lastPoint = points.Find((Vector2 p) => Math.Abs(p.x - point.x) < dx / 2);
        points.Remove(lastPoint);

        points.Add(point);
        points.Sort((Vector2 a, Vector2 b) => a.x < b.x ? -1 : 1);
        
        SetVerticesDirty();
    }

    public bool InRange(Vector2 p)
    {
        return p.x <= rangeX.y && p.x >= rangeX.x && p.y <= rangeY.y && p.y >= rangeY.x;
    }
    
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        if (points.Count < 2)
            return;
        
        for (int i = 0; i < points.Count; i++)
        {
            Vector2 point = points[i];

            float angle = 0;
            if (i < points.Count - 1) angle = Vector2.SignedAngle(Vector2.up, points[i + 1] - point);
            else angle = Vector2.SignedAngle(Vector2.up, point - points[i - 1]);
            
            DrawVerticesForPoint(point, vh, angle);
        }

        for (int i = 0; i < points.Count - 1; i++)
        {
            int index = i * 2;
            vh.AddTriangle(index + 0, index + 1, index + 3);
            vh.AddTriangle(index + 3, index + 2, index + 0);
        }
    }

    private void DrawVerticesForPoint(Vector2 point, VertexHelper vh, float angle)
    {
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;
        
        Vector3 pointOnUI = new Vector3(
            point.x.Remap(rangeX.x, rangeX.y, -width / 2, width / 2),
            point.y.Remap(rangeY.x, rangeY.y, -height / 2, height / 2)
            );
        
        vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(thickness / 2f, 0) + pointOnUI;
        vh.AddVert(vertex);

        vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(-thickness / 2f, 0) + pointOnUI;
        vh.AddVert(vertex);
    }


    public void Clear()
    {
        points.Clear();
        
        //SetVerticesDirty();
    }
}

using UnityEngine;
using UnityEngine.UI;

public class UIGridRenderer : Graphic
{
    public Vector2Int gridSize = new Vector2Int(10, 10);
    public float thickness = 1;

    public float width;
    public float height;
    
    private float cellWidth;
    private float cellHeight;
    
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        width = rectTransform.rect.width;
        height = rectTransform.rect.height;
        
        cellWidth = width / gridSize.x;
        cellHeight = height / gridSize.y;

        int count = 0;
        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                DrawCell(x,y,count,vh);
                count++;
            }
        }
    }

    private void DrawCell(int x, int y, int index, VertexHelper vh)
    {
        float xPos = cellWidth * x - width / 2;
        float yPos = cellHeight * y - height / 2;
        
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;


        vertex.position = new Vector3(xPos, yPos);
        vh.AddVert(vertex);
        
        vertex.position = new Vector3(xPos, yPos + cellHeight);
        vh.AddVert(vertex);
        
        vertex.position = new Vector3(xPos + cellWidth, yPos + cellHeight);
        vh.AddVert(vertex);
        
        vertex.position = new Vector3(xPos + cellWidth, yPos);
        vh.AddVert(vertex);
        
        
        float widthSqr = thickness * thickness;
        float distanceSqr = widthSqr / 2f;
        float distance = Mathf.Sqrt(distanceSqr);

        vertex.position = new Vector3(xPos + distance, yPos + distance);
        vh.AddVert(vertex);

        vertex.position = new Vector3(xPos + distance, yPos + cellHeight - distance);
        vh.AddVert(vertex);
        
        vertex.position = new Vector3(xPos + cellWidth - distance, yPos + cellHeight - distance);
        vh.AddVert(vertex);
        
        vertex.position = new Vector3(xPos + cellWidth - distance, yPos + distance);
        vh.AddVert(vertex);

        int offset = index * 8;
        
        // LEFT
        vh.AddTriangle(offset + 0, offset + 1, offset + 5);
        vh.AddTriangle(offset + 5, offset + 4, offset + 0);
        
        //TOP
        // vh.AddTriangle(offset + 1, offset + 2, offset + 6);
        // vh.AddTriangle(offset + 6, offset + 5, offset + 1);
        
        // RIGHT
        // vh.AddTriangle(offset + 2, offset + 3, offset + 7);
        // vh.AddTriangle(offset + 7, offset + 6, offset + 2);
        
        // BOTTOM
        vh.AddTriangle(offset + 3,  offset + 0,  offset + 4);
        vh.AddTriangle(offset + 4,  offset + 7,  offset + 3);
    }
}

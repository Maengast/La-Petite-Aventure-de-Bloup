using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid<Obj>
{

    LayerMask unwalkableMask;
    Obj[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;
    Vector2 worldSize;
    float nodeRadius;
    Vector3 startPosition;

    public Grid(Vector2 _worldSize, float _cellSize, Vector3 _startPosition , LayerMask wall,Func< bool, Vector3, int, int, Obj> createGridObject){
        nodeRadius = _cellSize;
        nodeDiameter = _cellSize * 2;
        worldSize = _worldSize;
        startPosition = _startPosition;
        gridSizeX = Mathf.RoundToInt(worldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(worldSize.y / nodeDiameter);
        unwalkableMask = wall;
        CreateGrid(createGridObject);
    }


    void CreateGrid(Func<bool, Vector3, int, int, Obj> createGridObject)
    {
        grid = new Obj[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = startPosition - Vector3.right * worldSize.x / 2 - Vector3.up * worldSize.y / 2 ;

        // Construct Grid
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
               Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
                // Check if there are obstacles
               bool walkable = !(Physics2D.OverlapCircle(worldPoint, nodeDiameter, unwalkableMask));
               grid[x, y] = createGridObject(walkable, worldPoint ,x, y);
            }
        }

    }


    public Obj[,] GetGrid()
    {
        return grid;
    }

    public Vector2 GetLenght()
    {
        return worldSize;
    } 


    public int GetWidth()
    {
        return gridSizeX;
    }

    public int GetHeight()
    {
        return gridSizeY;
    }
    public Obj GetGridObject(int x, int y)
    {
        return grid[x, y];
    }

    public Obj NodeFromWorldPoint(Vector3 _worldPosition)
    {
        float percentX = (_worldPosition.x + (worldSize.x / 2)) / worldSize.x;
        float percentY = (_worldPosition.y + worldSize.y / 2) / worldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }


}

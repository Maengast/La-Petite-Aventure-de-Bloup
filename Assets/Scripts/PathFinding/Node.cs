using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool IsWalkable;
    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;
    public Node Parent;
    public Vector3 Position;

    public Node(bool _walkable, Vector3 _pos ,int _gridX, int _gridY)
    {
        IsWalkable = _walkable;
        gridX = _gridX;
        gridY = _gridY;
        Position = _pos;
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }


}

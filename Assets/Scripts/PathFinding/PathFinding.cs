using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    public Grid<Node> grid;
    public Vector2 GridWorldSize;
    public float CellSize;
    public Vector3 StartPosition;
    public LayerMask wall;

    private void Start()
    {
        Func<bool, Vector3, int, int, Node> createNode = (bool walkable, Vector3 _pos, int x, int y) => new Node(walkable, _pos, x, y);
        grid = new Grid<Node>(GridWorldSize, CellSize, StartPosition,wall ,createNode);
    }

    public Grid<Node> GetGrid()
    {
        return grid;
    }

    public List<Node> FindPath(Vector3 _startPos, Vector3 _targetPos)
    {
        Node startNode = grid.NodeFromWorldPoint(_startPos);
        Node targetNode = grid.NodeFromWorldPoint(_targetPos);
        List<Node> openSet = new List<Node> { startNode };
        HashSet<Node> closedSet = new HashSet<Node>();
        while (openSet.Count > 0)
        {
            Node node = GetLowestFCostNode(openSet);

            if (node == targetNode)
            {
                return RetracePath(startNode ,targetNode);
               
            }

            openSet.Remove(node);
            closedSet.Add(node);

            List<Node> actualNodeNeighbours = GetNeighbours(node);
            foreach (Node neighbour in actualNodeNeighbours)
            {
                if (!neighbour.IsWalkable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.Parent = node;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
        return null;
    }


    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        // Check Est and West cells
        for (int x = -1; x <= 1; x++)
        {
            // Check north and South cells
            for (int y = -1; y <= 1; y++)
            {
                // Check if examining current cell
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < grid.GetWidth() && checkY >= 0 && checkY < grid.GetHeight())
                {
                    neighbours.Add(grid.GetGridObject(checkX, checkY));
                }
            }
        }

        return neighbours;
    }


    List<Node> RetracePath(Node _startNode,Node _endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = _endNode;
        while (currentNode != _startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }
        path.Reverse();
        return path;

    }

    private Node GetLowestFCostNode(List<Node> pathNodeList)
    {
        Node lowestFCostNode = pathNodeList[0];
        for (int i = 1; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = pathNodeList[i];
            }
        }
        return lowestFCostNode;
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
        return dstX + dstY;
        //int remaining = Mathf.Abs(dstX - dstY);
        //return 14 * Mathf.Min(dstX, dstY) + 10 * remaining;
        //if (dstX > dstY)
        //    return 14 * dstY + 10 * (dstX - dstY);
        //return 14 * dstX + 10 * (dstY - dstX);
    }

}

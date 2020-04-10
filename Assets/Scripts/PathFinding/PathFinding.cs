using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathFinder
{
    public class PathFinding : MonoBehaviour
    {
        private Grid Grid;
        public delegate void OnPathCompleteDelegate(List<Node> path);
        public bool IsDone;

        private void Start()
        {
            Grid = GetComponent<Grid>();
            IsDone = true;
        }

        public Grid GetGrid()
        {
            return Grid;
        }
        RaycastHit2D LedgeCheck(Vector3 pos)
        {
            RaycastHit2D ledgeHit = Physics2D.Raycast(
                pos,
                -Vector2.up,
                10,
                Grid.UnwalkableMask);

            return ledgeHit;
        }

        bool IsGrounded(Vector3 pos)
        {
            RaycastHit2D hit = LedgeCheck(pos);
            if (!hit.collider) return false;

            // Get the current node
            Node node = Grid.NodeFromWorldPoint(hit.point);
            //return false if object position is not grounded
            if (!node.Ledge) return false;

            return true;
        }

        public void FindPath(Vector3 startPos, Vector3 targetPos, OnPathCompleteDelegate pathComplete )
        {
            IsDone = false;

            Node startNode = Grid.NodeFromWorldPoint(LedgeCheck(startPos).point);
            Node targetNode = Grid.NodeFromWorldPoint(LedgeCheck(targetPos).point);

            List<Node> openList = new List<Node> { startNode };
            HashSet<Node> closedSet = new HashSet<Node>();
            while (openList.Count > 0)
            {
                // Get node with the lowest f cost of the actual openset
                Node node = GetLowestFCostNode(openList);
                if (node == targetNode) // check if actual node correspond to target node
                {
                    IsDone = true;
                    // if yes, build final path
                    pathComplete(RetracePath(startNode, targetNode));
                }

                openList.Remove(node); // Remove actual node from the open set
                closedSet.Add(node); // And then add it to the closed set
                List<Link> links = node.Links;

                foreach (Link nodeLink in links)
                {
                    // Check if node is a wall or if closed Set already contains this node
                    if (!nodeLink.target.IsWalkable || closedSet.Contains(nodeLink.target))
                    {
                        continue; // If true we continue to the next neighbour nodelink target
                    }
                    int newCostToNeighbour = node.GCost + GetDistance(node, nodeLink.target) + nodeLink.GetCost();
                    if (newCostToNeighbour < nodeLink.target.GCost || !openList.Contains(nodeLink.target))
                    {
                        // Update target node cost, parent and link type
                        nodeLink.target.GCost = newCostToNeighbour;
                        nodeLink.target.HCost = GetDistance(nodeLink.target, targetNode);
                        nodeLink.target.Parent = node;
                        nodeLink.target.LinkType = nodeLink.type;
                        // Add target node to open Set
                        if (!openList.Contains(nodeLink.target))
                            openList.Add(nodeLink.target);
                    }
                }

            }
            IsDone = true;
        }

        List<Node> RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;
            // Get final path
            while (currentNode != startNode)
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
            // Foreach node in actual open nodes
            for (int i = 1; i < pathNodeList.Count; i++)
            {
                if (pathNodeList[i].FCost < lowestFCostNode.FCost)
                {
                    lowestFCostNode = pathNodeList[i];
                }
            }
            // Return node with the lowest f cost
            return lowestFCostNode;
        }

        int GetDistance(Node currentNode, Node targetNode)
        {
            int dstX = Mathf.Abs(targetNode.GridX - currentNode.GridX);
            int dstY = Mathf.Abs(targetNode.GridY - currentNode.GridY);
            return dstX + dstY;
        }

    }
}


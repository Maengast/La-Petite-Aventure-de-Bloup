using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathFinder
{
    public class Node
    {
        public bool IsWalkable;
        public int GridX;
        public int GridY;

        public int GCost;
        public int HCost;
        public Node Parent;
        public Vector3 Position;
        public List<Link> Links = new List<Link>();
        public bool Ledge { get; set; }
        public PathLinkType LinkType = PathLinkType.none;

        public Node(bool _walkable, Vector3 _pos, int _gridX, int _gridY)
        {
            IsWalkable = _walkable;
            GridX = _gridX;
            GridY = _gridY;
            Position = _pos;
        }

        public int FCost
        {
            get
            {
                return GCost + HCost;
            }
        }

        public void AddLink(Node target, int distance, PathLinkType type = PathLinkType.ground)
        {
            Link link = new Link(target, type, distance);
            Links.Add(link);
        }
    }
}


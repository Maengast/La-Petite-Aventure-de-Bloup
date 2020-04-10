using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathFinder
{
    public enum PathLinkType: int
    {
        ground = 0, 
        runoff = 1,
        fall = 2,
        jump = 3
    }
    public class Link
    {

        public Node target;
        public int distance;
        public PathLinkType type = PathLinkType.ground;

        public Link(Node targetLink, PathLinkType linkType, int linkDistance)
        {
            target = targetLink;
            type = linkType;
            distance = linkDistance;
        }


        // Retrieve the total value of weight and distance
        public int GetCost()
        {
            return (int)type + distance;
        }
    }

}

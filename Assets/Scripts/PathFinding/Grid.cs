using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathFinder
{
    public class Grid : MonoBehaviour
    {

        public LayerMask UnwalkableMask;
        public LayerMask JumpMask;
        Node[,] GridArray;

        int GridSizeX, GridSizeY;
        Vector2 BoxPosition;
        public float CellSize;

        List<Node> Ledges = new List<Node>();
        List<Node> Corners = new List<Node>();

        public float JumpDistance = 5;
        float BoxWidth, BoxHeight;
        GameObject AreaBox;
        private void Start()
        {
            DefineBox();
            CreateGrid();
        }

        void DefineBox()
        {

            AreaBox = GameObject.Find("Background");
            BoxWidth = AreaBox.GetComponent<SpriteRenderer>().bounds.size.x;
            BoxHeight = AreaBox.GetComponent<SpriteRenderer>().bounds.size.y;
            float boxX = AreaBox.GetComponent<SpriteRenderer>().bounds.center.x - (AreaBox.GetComponent<SpriteRenderer>().bounds.size.x / 2);
            float boxY = AreaBox.GetComponent<SpriteRenderer>().bounds.center.y + (AreaBox.GetComponent<SpriteRenderer>().bounds.size.y / 2);
            BoxPosition = new Vector2(boxX, boxY);
        }

        // Construct Grid
        void CreateGrid()
        {
            GridSizeX = Mathf.RoundToInt(BoxWidth / CellSize);
            GridSizeY = Mathf.RoundToInt(BoxHeight / CellSize);
            GridArray = new Node[GridSizeX, GridSizeY];

            //
            // Start by creating nodes
            //
            CreateNodes();

            //
            // Then get ledges and corners nodes in the grid 
            //
            AddLedgesAndCorners();

            //
            // For each ledge, add a link to all ledge neighbors
            //
            AddWalkLinks();

            // Find ledge corner fall points
            // Raycast at specific angle to check fall point
            // If valid create a one way link
            //
            // Get jumpoints of each corners
            // Check if there are jump points
            // Raycast at specific angle to check jump point
            // If valid create jump link
            AddJumpAndFallLinksToCorners();

        }

        public bool OutOfBounds(int x, int y)
        {
            return x < 0 || x >= GetWidth() ||
                y < 0 || y >= GetHeight();
        }

        public bool Blocked(int x, int y)
        {
            if (OutOfBounds(x, y)) return true;
            if (!GridArray[x,y].IsWalkable) return true;

            return false;
        }

        private void CreateNodes()
        {
            for (int x = 0; x < GridSizeX; x++)
            {
                for (int y = 0; y < GridSizeY; y++)
                {
                    Vector3 nodePosition = new Vector3((x * CellSize) + BoxPosition.x + (CellSize / 2), -(y * CellSize) + BoxPosition.y + (CellSize / 2), 0);

                    // Check if there are obstacles
                    bool walkable = IsNodeWalkable(nodePosition);

                    // Create new node
                    GridArray[x, y] = new Node(walkable, nodePosition, x, y);
                }
            }
        }

        private bool IsNodeWalkable(Vector3 position)
        {
            // Get top left position of node
            Vector2 topLeft = new Vector2(position.x - (CellSize / 2), position.y - (CellSize / 2));

            // Get bottom right position of node
            Vector2 bottomRight = new Vector2(position.x + (CellSize / 2), position.y + (CellSize / 2));

            // Check if an unwalkable surface stands between these two points
            return !(Physics2D.OverlapArea(topLeft, bottomRight, UnwalkableMask));
        } 

    
        private void AddLedgesAndCorners()
        {
            for (int x = 0; x < GridSizeX; x++)
            {
                for (int y = 0; y < GridSizeY; y++)
                {
                    Node node = GetGridObject(x, y);
                    if (node.IsWalkable && Blocked(x, y + 1) && y + 1 < GridSizeY)
                    {   // If the bottom ledge is blocked or bottom of y axis
                        node.Ledge = true;
                        Ledges.Add(node);
                        if (!Blocked(x - 1, y + 1) || !Blocked(x + 1, y + 1))
                            Corners.Add(node);
                    }
                }
            }
        }

        private void AddWalkLinks()
        {
            foreach (Node ledge in Ledges)
            {
                int x = ledge.GridX;
                int y = ledge.GridY;

                // Check left and right neighbor
                for (int index = -1; index <= 1; index++)
                {

                    // Check if we are looking current cell
                    if (index == 0) { continue; /*If yes jump to next cell*/ }
                    if (!Blocked(x + index, y))
                    {
                        Node node = GetGridObject(x + index, y);
                        // if neighbour node is a ledge, add a walk link between ledge and neighbour node
                        if (node.Ledge) ledge.AddLink(node, 1);

                    }

                }
            }

        }

        private void AddJumpAndFallLinksToCorners()
        {
           
            foreach (Node corner in Corners)
            {
                // Discover the direction the tile is facing
                int direction = Blocked(corner.GridX - 1, corner.GridY + 1) ? 1 : -1;

                // Step over the facing direction 1 tile
                Node overhang = GetGridObject(corner.GridX + direction, corner.GridY);

                // Shoot a raycast straight down to the end of the boundary to look for a fall
                RaycastHit2D hit = Physics2D.Raycast(
                    overhang.Position,
                    -Vector2.up,
                    (GetHeight() - overhang.GridY) * CellSize,
                    UnwalkableMask);

                // If we hit something add a fall link at the hit target
                if (hit.collider)
                {
                    Node node = NodeFromWorldPoint(hit.point);
                    int distance = (int)Mathf.Floor(Vector3.Distance(corner.Position, node.Position));
                    corner.AddLink(node, distance, PathLinkType.fall);
                }

                float initialValue = 0.2f;
                float h = initialValue;
                /// Find all corner jump point
                while (h <= 0.5)
                {

                    hit = Physics2D.Raycast(
                        overhang.Position,
                        new Vector2( h*direction, -(initialValue + 0.5f) +h ),
                        JumpDistance,
                        JumpMask
                        );

                    h += 0.1f;
                    if (hit.collider)
                    {
                        Node node = NodeFromWorldPoint(hit.point);
                        int distance = (int)Mathf.Floor(Vector3.Distance(corner.Position, node.Position));
                        // platform corner to node link
                        corner.AddLink(node, distance, PathLinkType.jump);
                        // current node to platform corner
                        node.AddLink(corner, distance, PathLinkType.jump);
                    }

                }



            }
        }

        public Node[,] GetGridArray()
        {
            return GridArray;
        }

        public int GetWidth()
        {
            return GridSizeX;
        }

        public int GetHeight()
        {
            return GridSizeY;
        }
        public Node GetGridObject(int x, int y)
        {
            return GridArray[x, y];
        }

        public Node NodeFromWorldPoint(Vector3 worldPosition)
        {
            float percentX = Mathf.Abs(BoxPosition.x - worldPosition.x) / CellSize;
            float percentY = Mathf.Abs(BoxPosition.y - worldPosition.y) / CellSize;
            percentX = Mathf.Floor(percentX);
            percentY = Mathf.Floor(percentY);
            int x = Mathf.RoundToInt(percentX);
            int y = Mathf.RoundToInt(percentY);

            return GridArray[x, y];

        }

        private void OnDrawGizmos()
        {
 
                foreach (Node n in Corners)//Loop through every node in the grid
                {
                Gizmos.DrawLine(n.Position, new Vector3(0,0,0));//Draw the node at the position of the node.
                }
        }

    }


}



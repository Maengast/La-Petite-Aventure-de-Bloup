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
        private Node[,] _gridArray;

        private int _gridSizeX, _gridSizeY;
        private Vector2 _boxPosition;
        public float CellSize = 0.5f;

        private List<Node> _ledges = new List<Node>();
        private List<Node> _corners = new List<Node>();

        public float JumpDistance = 5;
        public float CornerOffset = 2;
        private float _boxWidth, _boxHeight;

        private Area _areaBox;
        
        public void InitGrid(Area gridArea, float cornerOffset, float jumpDistance)
        {
	        JumpDistance = jumpDistance;
	        CornerOffset = cornerOffset;
	        DefineBox(gridArea);
            CreateGrid();
        }

        void DefineBox(Area area)
        {
	        _areaBox = area;
            _boxWidth = area.Size.x;
            _boxHeight = area.Size.y;
            float boxX = area.Origin.x;
	        float boxY = area.Origin.y;
            _boxPosition = new Vector2(boxX, boxY);
        }

        // Construct Grid
        void CreateGrid()
        {
            _gridSizeX = Mathf.RoundToInt(_boxWidth / CellSize);
            _gridSizeY = Mathf.RoundToInt(_boxHeight / CellSize);
            _gridArray = new Node[_gridSizeX, _gridSizeY];

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
            if (!_gridArray[x,y].IsWalkable) return true;

            return false;
        }

        private void CreateNodes()
        {
            for (int x = 0; x < _gridSizeX; x++)
            {
                for (int y = 0; y < _gridSizeY; y++)
                {
                    Vector3 nodePosition = new Vector3((x * CellSize) + _boxPosition.x + (CellSize / 2), (y * CellSize) + _boxPosition.y + (CellSize / 2), 0);

                    // Check if there are obstacles
                    bool walkable = IsNodeWalkable(nodePosition);

                    // Create new node
                    _gridArray[x, y] = new Node(walkable, nodePosition, x, y);
                }
            }
        }

        private bool IsNodeWalkable(Vector3 position)
        {
            // Get top left position of node
            Vector2 topLeft = new Vector2(position.x - (CellSize / 2)+0.1f, position.y + (CellSize / 2)-0.1f);

            // Get bottom right position of node
            Vector2 bottomRight = new Vector2(position.x + (CellSize / 2)-0.1f, position.y - (CellSize / 2)+0.1f);

            // Check if an unwalkable surface stands between these two points
            return !(Physics2D.OverlapArea(topLeft, bottomRight, UnwalkableMask));
        } 

    
        private void AddLedgesAndCorners()
        {
            for (int x = 0; x < _gridSizeX; x++)
            {
                for (int y = 0; y < _gridSizeY; y++)
                {
                    Node node = GetGridObject(x, y);
                    if (node.IsWalkable && Blocked(x, y - 1) && y - 1 > 0)
                    {   // If the bottom ledge is blocked or bottom of y axis
	                    node.Ledge = true;
                        _ledges.Add(node);
                        if (!Blocked(x - 1, y - 1) || !Blocked(x + 1, y - 1))
                        {
	                        _corners.Add(node);
                        }
                    }
                }
            }
        }

        private void AddWalkLinks()
        {
            foreach (Node ledge in _ledges)
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
            foreach (Node corner in _corners)
            {
                // Discover the direction the tile is facing
                int direction = Blocked(corner.GridX - 1, corner.GridY - 1) ? 1 : -1;

                // Step over the facing direction 1 tile
                Node overhang = GetGridObject(corner.GridX + direction*(int)CornerOffset, corner.GridY);

                //Vector2 origin = new Vector2(overhang.Position.x + direction * CornerOffset, overhang.Position.y);
                // Shoot a raycast straight down to the end of the boundary to look for a fall
                RaycastHit2D hit = Physics2D.Raycast(
                    overhang.Position,
                    Vector2.down,
                    overhang.Position.y,
                    UnwalkableMask);

                // If we hit something add a fall link at the hit target
                if (hit.collider)
                {
                    Node node = NodeFromWorldPoint(hit.point);
                    int distance = (int)Mathf.Floor(Vector3.Distance(corner.Position, node.Position));
                    corner.AddLink(node, distance, PathLinkType.fall);
                }

                overhang = GetGridObject(corner.GridX + direction, corner.GridY);
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
                        GameObject test = new GameObject("jump");
                        test.transform.position = node.Position;
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
            return _gridArray;
        }

        public int GetWidth()
        {
            return _gridSizeX;
        }

        public int GetHeight()
        {
            return _gridSizeY;
        }
        public Node GetGridObject(int x, int y)
        {
            return _gridArray[x, y];
        }

        public Node NodeFromWorldPoint(Vector3 worldPosition)
        {
            float percentX = Mathf.Abs(_boxPosition.x - worldPosition.x) / CellSize;
            float percentY = Mathf.Abs(_boxPosition.y - worldPosition.y) / CellSize;
            int x = Mathf.RoundToInt(percentX);
            int y = Mathf.RoundToInt(percentY);

            return _gridArray[x, y];
        }
    }
}


